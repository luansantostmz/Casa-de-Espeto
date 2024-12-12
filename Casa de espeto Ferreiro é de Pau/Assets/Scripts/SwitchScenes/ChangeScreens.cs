using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScreens : MonoBehaviour
{
    [SerializeField] private Transform[] posItensForge;
	private Vector3[] initialPositionItensForge;

    [SerializeField] private Transform[] posItensAnvil;
	private Vector3[] initialPositionItensAnvil;

	[SerializeField] private Transform[] posItensStore;
	private Vector3[] initialPositionItensStore;

	private void Start()
	{
		initialPositionItensForge = new Vector3[posItensForge.Length];
		initialPositionItensAnvil = new Vector3[posItensAnvil.Length];
		initialPositionItensStore = new Vector3[posItensStore.Length];

		//Forge
		for (int i = 0; i < posItensForge.Length; i++)
		{
			if (posItensForge[i] != null) // Verifica se o objeto não está vazio
			{
				initialPositionItensForge[i] = posItensForge[i].position;
			}
		}
		//Anvil
		for (int i = 0; i < posItensAnvil.Length; i++)
		{
			if (posItensAnvil[i] != null) // Verifica se o objeto não está vazio
			{
				initialPositionItensAnvil[i] = posItensAnvil[i].position;
			}
		}
		//Store
		for (int i = 0; i < posItensStore.Length; i++)
		{
			if (posItensStore[i] != null) // Verifica se o objeto não está vazio
			{
				initialPositionItensStore[i] = posItensStore[i].position;
			}
		}

		MoverParaForaDaCamera();

		RestartPosAnvil();
	}

	public void RestartPosForge() 
	{
		MoverParaForaDaCamera();
		for (int i = 0; i < posItensForge.Length; i++)
		{
			if (posItensForge[i] != null)
			{
				posItensForge[i].position = initialPositionItensForge[i];
			}
		}		
	}
	public void RestartPosAnvil()
	{
		MoverParaForaDaCamera();

		for (int i = 0; i < posItensAnvil.Length; i++)
		{
			if (posItensAnvil[i] != null)
			{
				posItensAnvil[i].position = initialPositionItensAnvil[i];
			}
		}		
	}
	public void RestartPosStore()
	{
		MoverParaForaDaCamera();

		for (int i = 0; i < posItensStore.Length; i++)
		{
			if (posItensStore[i] != null)
			{
				posItensStore[i].position = initialPositionItensStore[i];
			}
		}
	}
	public void MoverParaForaDaCamera()
	{
		Camera cameraPrincipal = Camera.main; // Obtém a câmera principal
		if (cameraPrincipal == null) return; // Verifica se a câmera existe

		Vector3 foraDaCamera = cameraPrincipal.ViewportToWorldPoint(new Vector3(1000f, 1000f, cameraPrincipal.nearClipPlane + 10f));

		//Forge
		for (int i = 0; i < posItensForge.Length; i++)
		{
			if (posItensForge[i] != null)
			{
				posItensForge[i].position = foraDaCamera;
			}
		}
		//Anvil
		for (int i = 0; i < posItensAnvil.Length; i++)
		{
			if (posItensAnvil[i] != null)
			{
				posItensAnvil[i].position = foraDaCamera;
			}
		}
		//Store
		for (int i = 0; i < posItensStore.Length; i++)
		{
			if (posItensStore[i] != null)
			{
				posItensStore[i].position = foraDaCamera;
			}
		}
	}
}
