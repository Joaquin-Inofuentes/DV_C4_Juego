using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Control simple para el panel ConfirmDialog.
/// Asignar botones Yes/No desde el inspector.
/// </summary>
public class MA_P2_SM_ModuleYesOrNot : MonoBehaviour
{
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    private string screenName;

    private void Awake()
    {
        screenName = gameObject.name;
        if (yesButton != null) yesButton.onClick.AddListener(OnYes);
        if (noButton != null) noButton.onClick.AddListener(OnNo);
    }

    private void OnDestroy()
    {
        if (yesButton != null) yesButton.onClick.RemoveListener(OnYes);
        if (noButton != null) noButton.onClick.RemoveListener(OnNo);
    }

    private void OnYes()
    {
        // result 1 = yes
        MA_P2_SM_Controller.Instance.SendConfirmResponse(screenName, 1);
    }

    private void OnNo()
    {
        // result 0 = no
        MA_P2_SM_Controller.Instance.SendConfirmResponse(screenName, 0);
    }
}
