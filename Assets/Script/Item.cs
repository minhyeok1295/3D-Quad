using UnityEngine;

public class Item : MonoBehaviour
{

    public enum Type {
        Ammo, Coin, Grenade, Heart, Weapon,
    }

    public Type type;
    public int value;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * 10 * Time.deltaTime);
    }
}
