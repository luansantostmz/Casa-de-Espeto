using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MusicManager))]
public class MusicManagerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		// Desenha o Inspector padr�o
		DrawDefaultInspector();

		// Adiciona espa�o no layout
		GUILayout.Space(10);

		// Cria um estilo personalizado para o bot�o
		GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
		buttonStyle.fontSize = 14;       // Define o tamanho da fonte
		buttonStyle.fixedHeight = 40;   // Altura fixa
		buttonStyle.fixedWidth = 200;   // Largura fixa
		buttonStyle.normal.textColor = Color.white; // Cor do texto

		// Adiciona o bot�o ao Inspector
		GUILayout.BeginHorizontal(); // Centraliza o bot�o
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Pular M�sica", buttonStyle))
		{
			MusicManager musicManager = (MusicManager)target;
			musicManager.SkipTrack();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}
}
