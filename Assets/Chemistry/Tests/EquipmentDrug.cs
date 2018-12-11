using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chemistry.Equipments.Data;
using MagiCloud.Core.Events;

public class EquipmentDrug : MonoBehaviour {

    public EquipmentDrugInfo equipmentDrugInfo;

    private void Start()
    {
        gameObject.AddUpdateObject(UpdateObject);
    }

    void UpdateObject(GameObject target, Vector3 position, Quaternion quaternion, int handIndex)
    {
        Debug.Log("物体");
    }
}
