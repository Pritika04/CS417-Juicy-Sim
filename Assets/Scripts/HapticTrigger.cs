using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors; 

public class HapticTrigger : MonoBehaviour
{
    [Range(0, 1)] public float intensity = 0.5f;
    public float duration = 0.2f;

    public void TriggerHaptic(BaseInteractionEventArgs eventArgs)
    {
        var interactor = eventArgs.interactorObject as XRBaseInteractor;

        if (interactor != null)
        {
            var controller = interactor.GetComponentInParent<XRBaseController>();

            if (controller != null)
            {
                controller.SendHapticImpulse(intensity, duration);
            }
        }
    }
}