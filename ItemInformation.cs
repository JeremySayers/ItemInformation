using BepInEx;
using IL.RoR2.UI;
using On.RoR2.UI;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.UI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using HGTextMeshProUGUI = RoR2.UI.HGTextMeshProUGUI;
using MPButton = RoR2.UI.MPButton;
using PickupPickerPanel = On.RoR2.UI.PickupPickerPanel;

namespace JeremySayers
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.JeremySayers.ItemInformation", "Item Information", "1.0.0")]
    public class ItemInformation : BaseUnityPlugin
    {
		public void Awake()
        {
            Chat.AddMessage("Loaded Item Information.");
			PickupPickerPanel.SetPickupOptions += new PickupPickerPanel.hook_SetPickupOptions(this.SetPickupOptionsHook);
			On.RoR2.UI.MPButton.OnPointerEnter += new On.RoR2.UI.MPButton.hook_OnPointerEnter(this.OnPointerEnter);
			On.RoR2.UI.MPButton.OnPointerExit += new On.RoR2.UI.MPButton.hook_OnPointerExit(this.OnPointerExit);
		}

		public void OnDestory()
        {
			PickupPickerPanel.SetPickupOptions -= new PickupPickerPanel.hook_SetPickupOptions(this.SetPickupOptionsHook);
		}

		private void OnPointerEnter(On.RoR2.UI.MPButton.orig_OnPointerEnter orig, RoR2.UI.MPButton self, PointerEventData eventData)
        {
			Chat.AddMessage("Button Enter");
			var gameObjects = self.transform.GetComponents(typeof(Component));

			if (gameObjects.Length < 1)
            {
				Chat.AddMessage("No components on button.");
			} 
			else
            {
				Chat.AddMessage("Gameobjects in button.");
				foreach (var g in gameObjects)
				{
					Chat.AddMessage(g.name + g.GetType().ToString());
				}
			}		

			//if (gameObject != null)
			//         {
			//	gameObject.GetComponent<HGTextMeshProUGUI>().alpha = 1.0f;
			//} else
			//         {
			//	Chat.AddMessage("Could not find text container.");
			//}

			orig.Invoke(self, eventData);
		}

		private void OnPointerExit(On.RoR2.UI.MPButton.orig_OnPointerExit orig, RoR2.UI.MPButton self, PointerEventData eventData)
		{
			Chat.AddMessage("Button Exit");
			var gameObject = self.transform.GetComponents<GameObject>().Where(c => c.name.StartsWith("TextContainer")).SingleOrDefault();

			if (gameObject != null)
			{
				gameObject.GetComponent<HGTextMeshProUGUI>().alpha = 0f;
			} else
            {
				Chat.AddMessage("Could not find text container.");
			}

			orig.Invoke(self, eventData);
		}

		private void SetPickupOptionsHook(On.RoR2.UI.PickupPickerPanel.orig_SetPickupOptions orig, RoR2.UI.PickupPickerPanel self, PickupPickerController.Option[] options)
		{
			orig.Invoke(self, options);
			bool flag = !PickupCatalog.GetPickupDef(options[0].pickupIndex).equipmentIndex.Equals(EquipmentIndex.None);
			if (!flag)
			{
				ReadOnlyCollection<MPButton> elements = Reflection.GetFieldValue<UIElementAllocator<MPButton>>(self, "buttonAllocator").elements;
				for (int i = 0; i < options.Length; i++)
				{
					PickupDef pickupDef = PickupCatalog.GetPickupDef(options[i].pickupIndex);		
					GameObject gameObject = new GameObject("TextContainer_" + i.ToString());

					gameObject.transform.SetParent(elements[i].transform);
					gameObject.AddComponent<CanvasRenderer>();
					RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
					HGTextMeshProUGUI hgtextMeshProUGUI = gameObject.AddComponent<HGTextMeshProUGUI>();
					hgtextMeshProUGUI.text = pickupDef.nameToken;
					hgtextMeshProUGUI.fontSize = 30f;
					hgtextMeshProUGUI.color = Color.white;
					hgtextMeshProUGUI.alignment = TextAlignmentOptions.TopRight;
					hgtextMeshProUGUI.enableWordWrapping = false;
					hgtextMeshProUGUI.alpha = 0f;
					rectTransform.localPosition = Vector2.zero;
					rectTransform.anchorMin = Vector2.zero;
					rectTransform.anchorMax = Vector2.one;
					rectTransform.localScale = Vector3.one;
					rectTransform.sizeDelta = Vector2.zero;
					rectTransform.anchoredPosition = new Vector2(-5f, -1.5f);
					rectTransform.ForceUpdateRectTransforms();
				}
			}
		}
	}
}