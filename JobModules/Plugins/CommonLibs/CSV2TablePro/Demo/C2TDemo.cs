using UnityEngine;
using UnityEngine.UI;

public class C2TDemo : MonoBehaviour 
{
	public TextAsset csv;
	public GridLayoutGroup grid;
	public GameObject itemPrefab;

	public void Start()
	{
		Refresh();
	}

	public void Refresh()
	{
		if(csv == null)
			return;

		DemoTable table = new DemoTable();
		table.Load(csv.text);

		// names
		AddItem("Year", Color.gray);
		AddItem("Make", Color.gray);
		AddItem("Model", Color.gray);
		AddItem("Desc", Color.gray);
		AddItem("Price", Color.gray);

		// types
		DemoTable.Row row = table.GetAt(0);
		AddItem(row.Year.GetType().ToString(), Color.gray);
		AddItem(row.Make.GetType().ToString(), Color.gray);
		AddItem(row.Model.GetType().ToString(), Color.gray);
		AddItem(row.Description.GetType().ToString(), Color.gray);
		AddItem(row.Price.GetType().ToString(), Color.gray);

		// datas
		int numRows = table.NumRows();
		for(int i = 0 ; i < numRows ; i++)
		{
			AddItem(table.GetAt(i).Year);
			AddItem(table.GetAt(i).Make);
			AddItem(table.GetAt(i).Model);
			AddItem(table.GetAt(i).Description);
			AddItem(table.GetAt(i).Price.ToString());
		}
	}

	Text AddItem(string item)
	{
		return AddItem(item, Color.white);
	}

	Text AddItem(string item, Color color)
	{
		GameObject go = Instantiate(itemPrefab);
		go.transform.SetParent(grid.transform);

		go.GetComponent<Image>().color = color;

		Text text = go.GetComponentInChildren<Text>();
		text.text = item;

		return text;
	}
}