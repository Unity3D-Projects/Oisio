﻿using UnityEngine;
using UnityEngine.UI;

public class UIInventoryItem : MonoBehaviour
{
    [SerializeField] private ConsumableType item;
    [SerializeField] private Agent inventoryCarrier;

    [Header("UI Dependecies")]
    [SerializeField] private Text itemCount;

    private CharacterInventoryComponent inventoryComponent;

	private void Start ()
    {
        inventoryComponent = inventoryCarrier.RequestComponent<CharacterInventoryComponent>();
	}

    private void Update ()
    {
        if (inventoryComponent == null) return;

        itemCount.text = inventoryComponent.GetItemStock(item).ToString();
	}
}