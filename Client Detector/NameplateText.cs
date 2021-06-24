using System;
using TMPro;
using UnityEngine;

namespace Client_Detector
{
	public class NameplateText
	{
		public NameplateText(VRCPlayer player, string Text, Vector2 position, Color TextColor)
		{
			this.player = player;
			Transform transform = player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/Main/Text Container/Name");
			this.gameObject = UnityEngine.Object.Instantiate<GameObject>(transform.gameObject, transform);
			this.gameObject.name = string.Format("{0}_{1}-{2}", Text, position.x, position.y);
			this.text = this.gameObject.GetComponent<TextMeshProUGUI>();
			this.SetText(Text);
			this.SetPosition(position);
			this.SetTextColor(TextColor);
		}

		public NameplateText(VRCPlayer player, string Text, Vector2 position) : this(player, Text, position, Color.white)
		{
		}

		public void SetActive(bool Active)
		{
			this.gameObject.SetActive(Active);
		}

		public void SetPosition(Vector2 position)
		{
			this.gameObject.transform.localPosition = position;
		}

		public void SetTextColor(Color color)
		{
			this.text.color = color;
		}

		public void SetText(string Text)
		{
			this.text.text = Text;
		}

		public void Destroy()
		{
			UnityEngine.Object.Destroy(this.gameObject);
		}

		public VRCPlayer player;

		public TextMeshProUGUI text;

		public GameObject gameObject;
	}
}
