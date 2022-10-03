using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterGun : MonoBehaviour
{
    [SerializeField] private float waterUsedPerSecond;
    [SerializeField] private float waterGeneratedPerSecon;
    [SerializeField] private float maxWaterAmount;
    [SerializeField] private float minWaterToShoot;
    [Space(20)]
    [SerializeField] private float maxTankImageHeight;
    [Header("Import")]
    [SerializeField] private GameObject waterProjectilePrefab;
    [SerializeField] private Transform shootPosition;
    [SerializeField] private Image waterTankImage;

    private WaterProjectile currentWaterProjectile;
    private float waterAmount = 0;
    //private List<Vector2> waterPositions
    void Update()
    {
        waterAmount = Mathf.Clamp(waterAmount + waterGeneratedPerSecon * Time.deltaTime, 0 , maxWaterAmount);
        if (waterAmount > minWaterToShoot && Input.GetMouseButton(0))
        {
            //waterAmount -= waterUsedPerSecond * Time.deltaTime;
            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }
            if (currentWaterProjectile != null)
            {
                waterAmount -= waterUsedPerSecond * Time.deltaTime;
                currentWaterProjectile.AddLength(shootPosition.position, Player.instance.controller.lastMovementDirection);
            }
        }
        else {
            UnbindProjectile();
        }
        RectTransformExtensions.SetHeight(waterTankImage.rectTransform, Mathf.Lerp(0, maxTankImageHeight, waterAmount / maxWaterAmount));
    }

    private void Start()
    {
        waterAmount = maxWaterAmount;
    }

    public void UnbindProjectile() {
        if (currentWaterProjectile != null) { currentWaterProjectile.Unparent(); }
        currentWaterProjectile = null;
    }

    private void Shoot() {
        GameObject currentProjectile = Instantiate(waterProjectilePrefab);
        currentWaterProjectile = currentProjectile.GetComponent<WaterProjectile>();
    }
}
