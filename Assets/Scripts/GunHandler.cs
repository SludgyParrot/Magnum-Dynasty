using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public sealed class GunHandler : MonoBehaviour
{
    [SerializeField]
    private InputActionAsset inputAction;

    [SerializeField]
    private InputActionReference shootInput;

    [SerializeField, Space(5)]
    private Transform gunNozzle;

    [SerializeField, Space(5)]
    private float minDamage, maxDamage;

    [SerializeField, Space(5)]
    private float power;

    [SerializeField, Space(5)]
    private float shootRange;

    [SerializeField, Space(5)]
    private LayerMask shootableLayer;

    [SerializeField, Space(5)]
    private float fireRate;

    [SerializeField, Space(5)]
    private int maxAmmoCount;

    [SerializeField, Space(5)]
    private UnityEvent<string> onAmmoCountEvent;

    public float Damage => Random.Range(minDamage, maxDamage);

    private Animator gunAnimator;

    private bool isShooting;

    private float nextFireTime;

    private int ammoCount;

    public string AmmoCountString => $"Ammo {ammoCount}/{maxAmmoCount}";

    private void Start()
    {
        inputAction.Enable();

        shootInput.action.performed += context => OnShootInput(context, true);
        shootInput.action.canceled += context => OnShootInput(context, false);

        gunAnimator = GetComponent<Animator>();

        onAmmoCountEvent?.Invoke(AmmoCountString);
    }

    private void OnShootInput(UnityEngine.InputSystem.InputAction.CallbackContext obj, bool performed)
        => isShooting = performed;

    private void Update()
    {
        if (!isShooting) return;

        if(Time.time > nextFireTime)
        {
            nextFireTime = Time.time * fireRate / 100;
            Shoot();
        }
    }

    private void Shoot()
    {
        if(ammoCount <= 0)
        {
            gunAnimator.SetTrigger("No Ammo");
            return;
        }

        gunAnimator.SetTrigger("Shoot");

        if (Physics.Raycast(gunNozzle.position, gunNozzle.forward, out RaycastHit hitInfo, shootRange, shootableLayer))
        {
            Debug.Log($"~Shooting at {hitInfo.transform.name}");
            Debug.DrawLine(gunNozzle.position, hitInfo.point, Color.blue);

            if (hitInfo.rigidbody != null)
                hitInfo.rigidbody.AddForce(-hitInfo.normal * power);

            Vehicle vehicle = hitInfo.transform.GetComponent<Vehicle>();

            if (vehicle == null) return;

            vehicle.TakeDamage(Damage);
        }

        ammoCount--;
        onAmmoCountEvent?.Invoke(AmmoCountString);
    }

    public void AddAmmo(int ammoCount)
    {
        if (this.ammoCount < maxAmmoCount)
            this.ammoCount += ammoCount;

        if(this.ammoCount >= maxAmmoCount)
            this.ammoCount = maxAmmoCount;

        onAmmoCountEvent?.Invoke(AmmoCountString);
    }
}
