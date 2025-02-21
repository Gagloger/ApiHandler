using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class HttpHandler : MonoBehaviour
{
    [SerializeField]
    private RawImage[] pictures;
    [SerializeField] private TextMeshProUGUI[] descriptions;
    int descIndex = 0;

    [SerializeField] private TextMeshProUGUI usernameText;
    [SerializeField] private TextMeshProUGUI inventoryText;
    [SerializeField]
    private string url = "https://rickandmortyapi.com/api/character";
    [SerializeField] private string myUrl = "https://my-json-server.typicode.com/Gagloger/APITesting";

    private void Start()
    {
        for (int i = 1; i < pictures.Length; i++)
        {
            pictures[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < descriptions.Length; i++)
        {
            descriptions[i].text = "";
        }
        descIndex = 0;
        usernameText.text = "Usuario: ";
        inventoryText.text = "inventory: \n";
    }
    public void SendRequest(int id)
    {
        
        for (int i = 1; i < pictures.Length; i++)
        {
            pictures[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < descriptions.Length; i++)
        {
            descriptions[i].text = "";
        }
        descIndex = 0;
        usernameText.text = "Usuario: ";
        inventoryText.text = "inventory: \n";
        StartCoroutine(GetCharacter(56+id, 0));
        StartCoroutine(GetUser(id));  
    }

// Obtener la image de la API de Rick and Morty
    IEnumerator GetCharacter(int id, int index)
    {
        UnityWebRequest www = UnityWebRequest.Get(url + "/" + id);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            if (www.responseCode == 200)
            {

                Personaje personaje = JsonUtility.FromJson<Personaje>(www.downloadHandler.text);

                StartCoroutine(GetImage(personaje.image,index));

            }
            else
            {
                string mensaje = "status:" + www.responseCode;
                mensaje += "\nErro: " + www.error;
                Debug.Log(mensaje);
            }
        }
    }

// Desde esta se obtiene los datos de mi API del usuario especificado (id)
    IEnumerator GetUser(int id)
    {
        UnityWebRequest www = UnityWebRequest.Get(myUrl + "/users/" + id);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            if (www.responseCode == 200)
            {

                User usuario = JsonUtility.FromJson<User>(www.downloadHandler.text);

                usernameText.text += usuario.username;
                Debug.Log ("Items en el inventario: " + usuario.inventory.Length);
                for (int i =0 ; i < usuario.inventory.Length; i++)
                {
                    pictures[i+1].gameObject.SetActive(true);
                }

                for (int i = 0; i < usuario.inventory.Length; i++)
                {
                    int item = usuario.inventory[i];
                    //Debug.Log("Item: " + item);
                    StartCoroutine(GetInventory(item));
                    
                }
                
                
            }
            else
            {
                string mensaje = "status:" + www.responseCode;
                mensaje += "\nErro: " + www.error;
                Debug.Log(mensaje);
            }
        }
    }

    IEnumerator GetInventory(int itemID){
        UnityWebRequest www = UnityWebRequest.Get(myUrl + "/items/" + itemID);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            if (www.responseCode == 200)
            {
                Items item = JsonUtility.FromJson<Items>(www.downloadHandler.text);
                inventoryText.text += item.name + "\n";
                descriptions[descIndex].text = item.description;
                descIndex++;
                StartCoroutine(GetCharacter(itemID,descIndex));
            }
            else
            {
                string mensaje = "status:" + www.responseCode;
                mensaje += "\nErro: " + www.error;
                Debug.Log(mensaje);
            }
        }
    }
    IEnumerator GetText()
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            if (www.responseCode == 200)
            {
                ListaDePersonajes personajes = JsonUtility.FromJson<ListaDePersonajes>(www.downloadHandler.text);
                foreach (var personaje in personajes.results)
                {
                    Debug.Log($"{personaje.id}: {personaje.name} es un {personaje.species}");

                }
            }
            else
            {
                string mensaje = "status:" + www.responseCode;
                mensaje += "Error" + www.error;
                Debug.Log(mensaje);
            }

        }
    }
    IEnumerator GetImage(string imageUrl, int index)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            var texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            pictures[index].texture = texture;
        }
    }


}

#region RickMortyAPI
[System.Serializable]
public class Personaje
{
    public int id;
    public string name;
    public string species;
    public string image;
}


[System.Serializable]
public class ListaDePersonajes
{
    public Personaje[] results;
}
#endregion

#region MyJsonServer
[System.Serializable]
public class User
{
    public int id;
    public string username;

    public int[] inventory;
}

[System.Serializable]
public class Items
{
    public int id;
    public string name;

    public string description;
    
}

#endregion