using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MusicManager))]
public class MusicManagerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		// Desenha o Inspector padrão
		DrawDefaultInspector();

		// Adiciona espaço no layout
		GUILayout.Space(10);

		// Cria um estilo personalizado para o botão
		GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
		buttonStyle.fontSize = 14;       // Define o tamanho da fonte
		buttonStyle.fixedHeight = 40;   // Altura fixa
		buttonStyle.fixedWidth = 200;   // Largura fixa
		buttonStyle.normal.textColor = Color.white; // Cor do texto

		// Adiciona o botão ao Inspector
		GUILayout.BeginHorizontal(); // Centraliza o botão
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Pular Música", buttonStyle))
		{
			MusicManager musicManager = (MusicManager)target;
			musicManager.SkipTrack();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}
}
