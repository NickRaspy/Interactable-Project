using UnityEngine;
using UnityEngine.Events;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Lever : MonoBehaviour
{
    private const float OffAngle = -45f;
    private const float OnAngle = -135f;
    private const float SwitchAngle = -90f;

    [SerializeField] private XRSimpleInteractable interactable;
    [SerializeField] private Transform pivot;
    [SerializeField, Min(0.01f)] private float handTravelForFullStroke = 0.5f;
    [SerializeField] private UnityEvent onSwitchedOn;

    private Transform _hand;
    private XROrigin _xrOrigin;
    private float _grabHandHeight;
    private float _grabLeverAngle;
    private float _leverAngle = OffAngle;

    private bool canUse;

    public bool IsOn { get; private set; }

    private void Awake()
    {
        pivot.localRotation = Quaternion.Euler(0f, 0f, _leverAngle);
    }

    private void OnEnable()
    {
        interactable.selectEntered.AddListener(OnGrab);
        interactable.selectExited.AddListener(OnRelease);
    }

    private void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnGrab);
        interactable.selectExited.RemoveListener(OnRelease);
        _hand = null;
        _xrOrigin = null;
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        _hand = args.interactorObject.GetAttachTransform(args.interactableObject);
        _xrOrigin = args.interactorObject.transform.GetComponentInParent<XROrigin>();
        if (_hand == null || _xrOrigin == null)
        {
            Debug.LogWarning("Lever needs a controller under an XR Origin.", this);
            _hand = null;
            _xrOrigin = null;
            return;
        }

        _grabHandHeight = GetHandHeight();
        _grabLeverAngle = _leverAngle;
    }

    private void Update()
    {
        if (!_hand || !canUse) return;

        var handTravel = GetHandHeight() - _grabHandHeight;
        var degreesPerMeter = (OffAngle - OnAngle) / Mathf.Max(0.01f, handTravelForFullStroke);
        _leverAngle = Mathf.Clamp(_grabLeverAngle + handTravel * degreesPerMeter, OnAngle, OffAngle);
        pivot.localRotation = Quaternion.Euler(0f, 0f, _leverAngle);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (_hand == null) return;
        _hand = null;
        _xrOrigin = null;

        var switchedOn = _leverAngle <= SwitchAngle;
        _leverAngle = switchedOn ? OnAngle : OffAngle;
        pivot.localRotation = Quaternion.Euler(0f, 0f, _leverAngle);

        var wasOn = IsOn;
        IsOn = switchedOn;

        if (wasOn || !switchedOn) return;
        
        onSwitchedOn?.Invoke();
        canUse = false;

    }

    private float GetHandHeight()
    {
        return _xrOrigin.transform.InverseTransformPoint(_hand.position).y;
    }

    public void AllowUse() => canUse = true;
}
