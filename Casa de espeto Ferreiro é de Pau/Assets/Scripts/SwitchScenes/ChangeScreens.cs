using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScreens : MonoBehaviour
{
	public Animator animator;

	[Header("Forge")]
    [SerializeField] private Transform[] posItensForge;
	private Vector3[] initialPositionItensForge;

	[Header("Anvil")]
    [SerializeField] private Transform[] posItensAnvil;
	private Vector3[] initialPositionItensAnvil;

	[Header("Orders")]
	[SerializeField] private GameObject ordersObject;

	private void Start()
	{
		initialPositionItensForge = new Vector3[posItensForge.Length];
		initialPositionItensAnvil = new Vector3[posItensAnvil.Length];

		//Forge
		for (int i = 0; i < posItensForge.Length; i++)
		{
			if (posItensForge[i] != null) // Verifica se o objeto n�o est� vazio
			{
				initialPositionItensForge[i] = posItensForge[i].position;
			}
		}
		//Anvil
		for (int i = 0; i < posItensAnvil.Length; i++)
		{
			if (posItensAnvil[i] != null) // Verifica se o objeto n�o est� vazio
			{
				initialPositionItensAnvil[i] = posItensAnvil[i].position;
			}
		}

		MoverParaForaDaCamera();
	}

	public void RestartPosForge() 
	{
		MoverParaForaDaCamera();
		Debug.Log("ficou a forja");
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

		Debug.Log("Ficou a bigorna");
		for (int i = 0; i < posItensAnvil.Length; i++)
		{
			if (posItensAnvil[i] != null)
			{
				posItensAnvil[i].position = initialPositionItensAnvil[i];
			}
		}
		
	}
	public void MoverParaForaDaCamera()
	{
		Debug.Log("Moveu todos para fora");

		Camera cameraPrincipal = Camera.main; // Obt�m a c�mera principal
		if (cameraPrincipal == null) return; // Verifica se a c�mera existe

		Vector3 foraDaCamera = cameraPrincipal.ViewportToWorldPoint(new Vector3(1.5f, 0.5f, cameraPrincipal.nearClipPlane + 10f));

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
	}

	public void ToggleAnimationOrders()
	{
		animator.SetBool("IsOpen", !animator.GetBool("IsOpen"));		
	}

}
