using CustomInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScreenManager (C - MVC)
/// - Push / Pop / HideAll
/// - Validación de nombres duplicados en Awake
/// - Protección contra destroyed GameObjects
/// - Ajuste de sibling index para que pantalla push quede arriba
/// - Eventos estáticos:
///     OnScreenChanged(string screenName, string action)
///     ConfirmResponseEvent(string screenName, int result)  (0=no,1=sí)
/// - autoPopConfirmDialog: si true, ScreenManager hace Pop cuando recibe confirm.
/// </summary>
public class MA_P2_SM_Controller : MonoBehaviour
{
    [Button(nameof(RefreshList))]
    [Header("Opciones")]
    [Tooltip("Si true, al recibir ConfirmResponseEvent el manager hace Pop() automático del confirm dialog.")]
    [SerializeField] public bool autoPopConfirmDialog = true;

    public static MA_P2_SM_Controller Instance { get; private set; }

    [Header("Asignar pantallas únicas (GameObjects hijos del Canvas)")]
    [SerializeField] public MA_P2_SM_Model[] screens;

    [Header("Referencia a la vista")]
    [SerializeField] public MA_P2_SM_View view;


    // Stack lógico de pantallas
    public Stack<MA_P2_SM_Model> screenStack = new Stack<MA_P2_SM_Model>();

    // Eventos públicos estáticos
    public static Action<string, string> OnScreenChanged;
    public static Action<string, int> ConfirmResponseEvent;

    // Estado público
    public bool InputBlocked { get; private set; } = false;

    // Mapa rápido por nombre (para búsquedas seguras)
    private Dictionary<string, MA_P2_SM_Model> screenMap = new Dictionary<string, MA_P2_SM_Model>();

    public void RefreshList()
    {
        // Actualiza la lista publica llamando a una inicializacion
        OnEnable();
    }

    private void OnEnable()
    {
        Instance = this;

        if (view == null)
        {
            view = GetComponent<MA_P2_SM_View>();
            if (view == null)
                Debug.LogError("Falta asociar el componente screen view");
        }
        List<MA_P2_SM_Model> autoScreens = new List<MA_P2_SM_Model>();

        foreach (Transform child in transform)
        {
            MA_P2_SM_Model newScreen = new MA_P2_SM_Model();
            newScreen.ScreenObject = child.gameObject;

            // Si el nombre contiene "Dialog" (mayúscula o minúscula), activa blocksInput
            if (child.name.IndexOf("Dialog", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                newScreen.blocksInput = true;
            }
            else
            {
                newScreen.blocksInput = false;
            }

            autoScreens.Add(newScreen);
        }

        // Asignar al array principal
        screens = autoScreens.ToArray();

        // Importante: Reconstruir el mapa interno para que el manager reconozca los nuevos datos
        BuildMapAndValidate();

        // Subscribe internal handler to ConfirmResponseEvent so manager can auto-pop optionally
        ConfirmResponseEvent += OnConfirmResponseInternal;
    }

    private void OnDestroy()
    {
        // limpiar suscripción interna
        ConfirmResponseEvent -= OnConfirmResponseInternal;
    }

    // -------------------------
    // Inicialización y validaciones
    // -------------------------
    private void BuildMapAndValidate()
    {
        screenMap.Clear();
        if (screens == null || screens.Length == 0)
        {
            Debug.LogWarning("[ScreenManager] No hay pantallas asignadas en inspector.");
            return;
        }

        var nameSet = new HashSet<string>();
        foreach (var s in screens)
        {
            if (s == null || s.ScreenObject == null)
            {
                Debug.LogWarning("[ScreenManager] Entry de pantalla nulo en array 'screens'. Ignorando.");
                continue;
            }

            string name = s.ScreenObject.name;
            if (nameSet.Contains(name))
            {
                Debug.LogError($"[ScreenManager] Error: pantalla duplicada con nombre '{name}'. Nombres deben ser únicos.");
                // no agregamos duplicado al map (dejamos el primero)
                continue;
            }

            nameSet.Add(name);
            screenMap[name] = s;
        }
    }

    // -------------------------
    // API pública
    // -------------------------

    /// <summary>
    /// Push: activa pantalla (overlay) y la apila. Si ya estaba en stack, la trae al tope.
    /// </summary>
    public void Push(string screenName)
    {
        if (string.IsNullOrEmpty(screenName)) return;

        // Limpia referencias rotas antes de operar
        CleanupDestroyedStackEntries();

        if (!screenMap.TryGetValue(screenName, out var screen))
        {
            Debug.LogWarning($"[ScreenManager] Push: no existe pantalla con nombre '{screenName}'");
            return;
        }

        if (screen.ScreenObject == null)
        {
            Debug.LogWarning($"[ScreenManager] Push: pantalla '{screenName}' tiene ScreenObject null (posible destroy).");
            return;
        }

        // Si ya es top, no duplicar
        if (screenStack.Count > 0 && screenStack.Peek() == screen)
        {
            // garantía visual: subir sibling aunque ya sea top
            BringToFront(screen);
            OnScreenChanged?.Invoke(screen.ScreenName, "Push (alreadyTop)");
            return;
        }

        // Si está en stack pero no top: remover la instancia antigua para traerla arriba
        if (screenStack.Contains(screen))
        {
            // reconstruir stack sin esa pantalla
            var tmp = new Stack<MA_P2_SM_Model>();
            while (screenStack.Count > 0)
            {
                var s = screenStack.Pop();
                if (s != screen) tmp.Push(s);
            }
            while (tmp.Count > 0) screenStack.Push(tmp.Pop());
            // ahora la pantalla quedó fuera y la podemos volver a apilar
        }

        // Activar y apilar
        view.ShowOverlay(screen);
        BringToFront(screen);
        screenStack.Push(screen);

        // Estado de bloqueo determinado por pantalla tope
        InputBlocked = screen.blocksInput;

        OnScreenChanged?.Invoke(screen.ScreenName, "Push");
    }

    /// <summary>
    /// Pop: cierra pantalla superior y restaura estado del siguiente.
    /// </summary>
    public void Pop()
    {
        CleanupDestroyedStackEntries();

        if (screenStack.Count == 0) return;

        var top = screenStack.Pop();
        if (top?.ScreenObject != null)
        {
            view.Hide(top);
            OnScreenChanged?.Invoke(top.ScreenName, "Pop");
        }
        else
        {
            // caso raro: top ya destruido
            OnScreenChanged?.Invoke(top?.ScreenName ?? "Unknown", "Pop(destroyed)");
        }

        // Restaurar InputBlocked en base al nuevo tope (si existe)
        if (screenStack.Count > 0)
        {
            var previous = screenStack.Peek();
            if (previous?.ScreenObject != null)
            {
                // aseguramos que esté activo (no forzamos sibling)
                view.ShowOverlay(previous);
                InputBlocked = previous.blocksInput;
                // no cambiamos sibling index del previous para respetar orden original
            }
            else
            {
                InputBlocked = false;
            }
        }
        else
        {
            InputBlocked = false;
        }
    }

    /// <summary>
    /// Oculta todas las pantallas y limpia stack.
    /// </summary>
    public void HideAll()
    {
        view.HideAll(screens);
        screenStack.Clear();
        InputBlocked = false;
        OnScreenChanged?.Invoke("AllScreens", "HideAll");
    }

    // -------------------------
    // Helpers y protecciones
    // -------------------------

    // Lleva el RectTransform al frente (SetAsLastSibling) si aplica.
    private void BringToFront(MA_P2_SM_Model screen)
    {
        if (screen?.ScreenObject == null) return;
        var rt = screen.ScreenObject.GetComponent<RectTransform>();
        if (rt != null)
            rt.SetAsLastSibling();
    }

    // Remueve del stack cualquier entrada cuyo GameObject haya sido destruido fuera del manager.
    private void CleanupDestroyedStackEntries()
    {
        if (screenStack.Count == 0) return;
        var tmp = new Stack<MA_P2_SM_Model>();
        bool hadNulls = false;

        while (screenStack.Count > 0)
        {
            var s = screenStack.Pop();
            if (s == null || s.ScreenObject == null)
            {
                hadNulls = true;
                // skip
            }
            else
            {
                tmp.Push(s);
            }
        }

        // Reconstruir stack orden original
        while (tmp.Count > 0) screenStack.Push(tmp.Pop());

        if (hadNulls)
            Debug.LogWarning("[ScreenManager] Cleanup: encontraron pantallas destruidas en stack y se removieron.");
    }

    // Buscar pantalla por nombre (segura)
    private MA_P2_SM_Model FindScreenByName(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        screenMap.TryGetValue(name, out var s);
        return s;
    }

    // Manejo interno a ConfirmResponseEvent para autopop opcional
    private void OnConfirmResponseInternal(string screenName, int result)
    {
        if (!autoPopConfirmDialog) return;

        // Si top del stack es la pantalla con screenName, pop it
        CleanupDestroyedStackEntries();
        if (screenStack.Count == 0) return;

        var top = screenStack.Peek();
        if (top != null && top.ScreenName == screenName)
        {
            Pop();
        }
    }

    // Permite disparar la confirmación desde fuera también
    public void SendConfirmResponse(string screenName, int result)
    {
        // valida
        if (string.IsNullOrEmpty(screenName)) return;
        ConfirmResponseEvent?.Invoke(screenName, result);
    }

    // Devuelve nombre de pantalla superior o null
    public string GetCurrentScreenName()
    {
        CleanupDestroyedStackEntries();
        return screenStack.Count > 0 ? screenStack.Peek().ScreenName : null;
    }
}
