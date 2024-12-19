using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class CSVDownloader : MonoBehaviour
{
	// URL do arquivo CSV
	private static string csvURL = "https://docs.google.com/spreadsheets/d/1kdne-qY0r5m3on7c05eRZwZ9ZJZX2AcNECntPOeWcLM/export?format=csv";

	// Caminho para salvar o arquivo localmente
	private static string localFilePath;

	[UnityEditor.MenuItem("Tools/Download CSV")]
	public static void DownloadCSVMenuItem()
	{
		// Define o caminho local para salvar o arquivo
		string localizationFolder = Path.Combine(Application.dataPath, "Localization");

		// Cria a pasta Localization caso ela não exista
		if (!Directory.Exists(localizationFolder))
		{
			Directory.CreateDirectory(localizationFolder);
		}

		localFilePath = Path.Combine(localizationFolder, "localization.csv");

		// Cria um GameObject temporário para executar a coroutine
		GameObject downloaderObject = new GameObject("CSVDownloader");
		CSVDownloader downloader = downloaderObject.AddComponent<CSVDownloader>();
		downloader.StartCoroutine(downloader.DownloadCSV());
	}

	private IEnumerator DownloadCSV()
	{
		using (UnityWebRequest webRequest = UnityWebRequest.Get(csvURL))
		{
			// Envia o request
			yield return webRequest.SendWebRequest();

			// Verifica se houve algum erro
			if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
				webRequest.result == UnityWebRequest.Result.ProtocolError)
			{
				Debug.LogError($"Erro ao baixar o arquivo: {webRequest.error}");
			}
			else
			{
				try
				{
					// Salva o arquivo localmente
					File.WriteAllBytes(localFilePath, webRequest.downloadHandler.data);
					Debug.Log($"Arquivo CSV baixado e salvo em: {localFilePath}");

					// Atualiza o banco de dados de Assets no Unity
#if UNITY_EDITOR
					UnityEditor.AssetDatabase.Refresh();
#endif
				}
				catch (System.Exception e)
				{
					Debug.LogError($"Erro ao salvar o arquivo: {e.Message}");
				}
			}

			// Remove o GameObject temporário
			DestroyImmediate(gameObject);
		}
	}
}