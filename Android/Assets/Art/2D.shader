Shader "2D" 
{

	Properties 
	{
	
		_MainTex ("Base (RGB)", 2D) = "white" {}
	
	}
	
	SubShader 
	{
	
		Pass 
		{
		
			SetTexture [_MainTex] 
			{
			
			Combine texture 
		
			}
		}
		
	}
	
}