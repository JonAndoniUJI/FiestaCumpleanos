using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class DocumentWritter : MonoBehaviour
{
    // Start is called before the first frame update
    string rutaDeEntrada;
    int numeroDeRespuestas;
    string nombreDeArchivo = "Documentation.txt";

    string separadorText = "=================================================";
    string encabezadoText = "Documentacion del caso: El Pepes";
    

    StreamWriter streamWritter;
    void Start()
    {
        if(File.Exists(Application.dataPath + "/Output/Documents/" + nombreDeArchivo))
        {

        }

        rutaDeEntrada = Application.dataPath + "/Output/Documents/" + nombreDeArchivo;
        streamWritter = new StreamWriter(rutaDeEntrada);

        streamWritter.WriteLine(separadorText);
        streamWritter.WriteLine(encabezadoText);
        streamWritter.WriteLine(separadorText);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void writeDocument(float initialSystemTime, float askingTime, float askingSystemTime, float answerTime, float initialAnswerSystemTime, float endingSystemTime, 
        float interruptionTime = 0, float interruptionSystemTime = 0)
    {
        streamWritter.WriteLine(separadorText);
        
        streamWritter.WriteLine("Numero de respuesta: " + numeroDeRespuestas + "\n");
        numeroDeRespuestas++;

        streamWritter.WriteLine("Tiempo de sistema (inicial): " + initialSystemTime.ToString("F2"));
        streamWritter.WriteLine("Tiempo de la pregunta: " + askingTime.ToString("F2") + " (" + askingSystemTime.ToString("F2") + ")");
        if (interruptionTime != 0)
            streamWritter.WriteLine("Respuesta antes de terminar: " + interruptionTime.ToString("F2") + " (" + interruptionSystemTime.ToString("F2") + ")");
        streamWritter.WriteLine("Tiempo de respuesta: " + answerTime.ToString("F2") + " (" + initialAnswerSystemTime.ToString("F2") + " - " + endingSystemTime.ToString("F2") + ")\n");
        
        streamWritter.WriteLine(separadorText);
    }

    public void closeDocument()
    {
        streamWritter.Close();
    }
}
