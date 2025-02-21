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
    private RawImage picture;

    [SerializeField] private TextMeshProUGUI usernameText;
    [SerializeField]
    private string url = "https://rickandmortyapi.com/api/character";
    private string myUrl = "https://my-json-server.typicode.com/Gagloger/APITesting/users";

    public void SendRequest(int id)
    {
        StartCoroutine(GetCharacter(56));
        StartCoroutine(GetUser(id));  
    }

// Obtener la image de la API de Rick and Morty
    IEnumerator GetCharacter(int id)
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

                StartCoroutine(GetImage(personaje.image));


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
        UnityWebRequest www = UnityWebRequest.Get(myUrl + "/" + id);
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

                usernameText.text = usuario.username;

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
    IEnumerator GetImage(string imageUrl)
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
            picture.texture = texture;
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

    public Inventory inventory;
}

[System.Serializable]
public class Inventory
{
    public int id;
    public string name;

    public List<int> items;
    //public List<Item> items;
}

#endregion