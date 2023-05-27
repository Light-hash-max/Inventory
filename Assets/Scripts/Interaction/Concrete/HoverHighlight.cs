using System.Collections.Generic;
using UnityEngine;

namespace LinkedSquad.Interactions.Effects
{
    public class HoverHighlight : MonoBehaviour
    {
        [Header("Hover Parameters")]
        [SerializeField] private Material _hoverMaterial;
        [SerializeField] private MeshRenderer _targetRenderer;



        public void OnHover() => SetHoverMaterial(_targetRenderer, _hoverMaterial);
        public void OnUnhover() => RemoveHoverMaterial(_targetRenderer, _hoverMaterial);

        private void SetHoverMaterial(MeshRenderer target, Material material)
        {
            MeshRenderer mesh = target != null
                ? target
                : GetComponent<MeshRenderer>();

            var mats = new List<Material>();

            mats.AddRange(mesh.sharedMaterials);
            mats.Add(material);

            mesh.sharedMaterials = mats.ToArray();
        }

        private void RemoveHoverMaterial(MeshRenderer target, Material material)
        {
            MeshRenderer mesh = target != null 
                ? target
                : GetComponent<MeshRenderer>();

            var mats = new List<Material>();

            mats.AddRange(mesh.sharedMaterials);
            mats.Remove(material);

            mesh.sharedMaterials = mats.ToArray();
        }
    }
}