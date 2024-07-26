using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using GorillaNetworking;
using Utilla.Models;
using TMPro;
using GorillaExtensions;

namespace Utilla
{
	public class GamemodeSelector : MonoBehaviour
	{
		const int PageSize = 4;

		ModeSelectButton[] modeSelectButtons = Array.Empty<ModeSelectButton>();

		List<TextMeshPro> gamemodesText = new List<TextMeshPro>();

        int page;

		public void Initialize(Transform parent, Transform buttonParent, List<GameObject> gamemodesList)
		{
			transform.parent = parent;

			var buttons = Enumerable.Range(0, PageSize).Select(x => buttonParent.GetChild(x));
			modeSelectButtons = buttons.Select(x => x.GetComponent<ModeSelectButton>()).ToArray();

			foreach (GameObject gamemode in gamemodesList)
			{
				TextMeshPro title = gamemode.GetComponentInChildren<TextMeshPro>();
                gamemodesText.Add(title);
				title.enabled = true;
				title.lineSpacing = 1.06f * 1.2f;
				title.transform.localScale *= 0.85f;
				title.transform.position += title.transform.right * 0.05f;
				title.overflowMode = TextOverflowModes.Overflow; // HorizontalWrapMode.Overflow;
			}

			CreatePageButtons(buttons.First().gameObject);

			ShowPage(0);
		}

		static GameObject fallbackTemplateButton = null;
		void CreatePageButtons(GameObject templateButton)
		{
			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.SetActive(false);
			MeshFilter meshFilter = cube.GetComponent<MeshFilter>();

			GameObject CreatePageButton(string text, Action onPressed)
			{
				GameObject button = GameObject.Instantiate(templateButton.transform.childCount == 0 ? fallbackTemplateButton : templateButton);
				button.GetComponent<MeshFilter>().mesh = meshFilter.mesh;
				button.GetComponent<Renderer>().material = templateButton.GetComponent<GorillaPressableButton>().unpressedMaterial;
				button.transform.parent = templateButton.transform.parent;
				button.transform.localRotation = templateButton.transform.localRotation;
				button.transform.localScale = Vector3.one * 0.1427168f; // shouldn't hurt anyone for now 

				button.transform.GetChild(0).gameObject.SetActive(true);
				button.transform.GetComponentInChildren<TextMeshPro>().gameObject.SetActive(false);

                Text buttonText = button.GetComponentInChildren<Text>();
				if (buttonText != null)
				{
					buttonText.text = text;
					buttonText.transform.localScale = Vector3.Scale(buttonText.transform.localScale, new Vector3(2, 2, 1));
				}

				GameObject.Destroy(button.GetComponent<ModeSelectButton>());
				button.AddComponent<PageButton>().onPressed += onPressed;

				if (!button.GetComponentInParent<Canvas>())
				{
					Canvas canvas = button.transform.parent.gameObject.GetOrAddComponent<Canvas>();
					canvas.renderMode = RenderMode.WorldSpace;
				}

				return button;
			}

			GameObject nextPageButton = CreatePageButton("-->", NextPage);
			nextPageButton.transform.localPosition = new Vector3(-0.775f, 0.0028f, -0.0104f);

			GameObject previousPageButton = CreatePageButton("<--", PreviousPage);
			previousPageButton.transform.localPosition = new Vector3(-0.775f, -0.618f, -0.0199f);

			Destroy(cube);

			if (templateButton.transform.childCount != 0)
			{
				fallbackTemplateButton = templateButton;
			}
		}

		public void NextPage()
		{
			if (page < GamemodeManager.Instance.PageCount - 1)
			{
				Debug.Log("pressed the next page");
				ShowPage(page + 1);
            }
		}

		public void PreviousPage()
		{
			if (page > 0)
			{
				Debug.Log("pressed the previous page");
				ShowPage(page - 1);
            }
		}

		void ShowPage(int page)
		{
			this.page = page;
			List<Gamemode> currentGamemodes = GamemodeManager.Instance.Gamemodes.Skip(page * PageSize).Take(PageSize).ToList();

            int counter = 0;
			for (int i = 0; i < modeSelectButtons.Length; i++)
			{
				if (i < currentGamemodes.Count)
				{
					modeSelectButtons[i].enabled = true;
					modeSelectButtons[i].gameMode = currentGamemodes[i].GamemodeString;
				}
				else
				{
					modeSelectButtons[i].enabled = false;
					modeSelectButtons[i].gameMode = "";
					counter++;
				}
			}

            foreach (TextMeshPro gamemodeText in gamemodesText)
            {
                gamemodeText.text = currentGamemodes[gamemodesText.IndexOf(gamemodeText)].DisplayName;
				gamemodeText.enableWordWrapping = false;
				RectTransform rectTransform = gamemodeText.rectTransform;
				rectTransform.localPosition = new Vector3(2.4763f, rectTransform.localPosition.y, rectTransform.localPosition.z);
				rectTransform.sizeDelta = new Vector2(100, rectTransform.sizeDelta.y);
				gamemodeText.fontSize = 65;
            }

            GorillaComputer.instance.OnModeSelectButtonPress(GorillaComputer.instance.currentGameMode.Value, GorillaComputer.instance.leftHanded);
        }
	}
}
