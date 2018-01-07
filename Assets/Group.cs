using UnityEngine;
using System.Collections;

public class Group : MonoBehaviour {
    public static Color[] colors = new Color[] { Color.red, Color.blue, Color.white, Color.yellow }; 
    float lastFall = 0;

	// Default position not valid? Board is filled up, Game Over.
	void Start () {
	    if(!isValidGridPos())  {
            Debug.Log("GAME OVER");
            Destroy(gameObject);
        }
       foreach(Transform child in transform) {
            int i = Random.Range(0, colors.Length);
            child.gameObject.GetComponent<SpriteRenderer>().color = colors[i];
        }
    }
	
	// Update is called once per frame
	void Update () {
        // move left
	    if(Input.GetKeyDown(KeyCode.LeftArrow)) {
            transform.position += new Vector3(-1, 0, 0);

            if (isValidGridPos())
                updateGrid();
            else
                // undo
                transform.position += new Vector3(1, 0, 0);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.position += new Vector3(1, 0, 0);

            if (isValidGridPos())
                updateGrid();
            else
                // undo
                transform.position += new Vector3(-1, 0, 0);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.Rotate(0, 0, -90);

            if (isValidGridPos())
                updateGrid();
            else
                // undo
                transform.Rotate(0, 0, 90);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) ||
            Time.time - lastFall >= 1)
        {
            transform.position += new Vector3(0, -1, 0);

            if (isValidGridPos())
                updateGrid();
            else {
                // undo
                transform.position += new Vector3(0, 1, 0);

                Grid.deleteFullRows();

                FindObjectOfType<Spawner>().spawnNext();
                enabled = false;
            }
            lastFall = Time.time;  
        }
    }

    bool isValidGridPos() {
        foreach(Transform child in transform) {
            Vector2 v = Grid.roundVec2(child.position);

            if (!Grid.insideBorder(v))
                return false;
            // block in grid cell and not part of this same group
            if (Grid.grid[(int)v.x, (int)v.y] != null &&
                Grid.grid[(int)v.x, (int)v.y].parent != transform)
                return false;
        }
        return true;
    }

    void updateGrid() {
        // remove all old children from grid
        for (int y = 0; y < Grid.h; ++y)
            for (int x = 0; x < Grid.w; ++x)
                if (Grid.grid[x, y] != null)
                    if (Grid.grid[x, y].parent == transform)
                        Grid.grid[x, y] = null;
        
        // add new children to grid
        foreach(Transform child in transform) {
            Vector2 v = Grid.roundVec2(child.position);
            Grid.grid[(int)v.x, (int)v.y] = child;
        }

    }
}
