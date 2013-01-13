float4x4 World;
float4x4 View;
float4x4 Projection;


float AmbientIntensity = 1;
int numlights = 20;
float4x4 WorldInverseTranspose;
float3 LightPositions[20];
float RadiusSizes[20];
float4 DiffuseColor = float4(1, 0, 1, 1);
float DiffuseIntensity = .9;
bool NV = false;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;    
    float4 Normal : NORMAL0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
	float4 FColor = input.Color;
    output.Position = mul(viewPosition, Projection);
	
	int cntr = 0;
	
	float3 DiffuseLightDirection=LightPositions[0];
	float Closest = 1000;
	float3 DiffuseLightDirectionStore;
	int spot = 0;
	float radius = 0;
	float storeRadius = 0;
	[loop]
	for(cntr = 0; cntr < numlights; cntr++)
	{
		storeRadius=RadiusSizes[cntr];
		DiffuseLightDirectionStore = LightPositions[cntr];
		DiffuseLightDirectionStore.x = DiffuseLightDirectionStore.x - input.Position.x;
		DiffuseLightDirectionStore.y = DiffuseLightDirectionStore.y - input.Position.y;
		DiffuseLightDirectionStore.z = DiffuseLightDirectionStore.z - input.Position.z;
		if(sqrt(pow(DiffuseLightDirectionStore.x, 2) + pow(DiffuseLightDirectionStore.y, 2) +pow(DiffuseLightDirectionStore.z, 2))-storeRadius*10<Closest)
		{
			DiffuseLightDirection = DiffuseLightDirectionStore;
			Closest=sqrt(pow(DiffuseLightDirection.x, 2) + pow(DiffuseLightDirection.y, 2) +pow(DiffuseLightDirection.z, 2))-storeRadius*10;
			radius = RadiusSizes[cntr];
		}
	}
	
	Closest = Closest + radius*10;
	//float4 normal = mul(input.Normal, WorldInverseTranspose);
	//dot(normal, DiffuseLightDirection))/100;
	float lightIntensity = (12 * radius - Closest) / (10 * radius);
	if(lightIntensity<0)
		lightIntensity=0;
	FColor = FColor * DiffuseIntensity * lightIntensity + float4(0,0,0,FColor.w * (1 - DiffuseIntensity * lightIntensity));
	if (NV==true)
		FColor = FColor * DiffuseIntensity * lightIntensity + float4(0,.4,0,FColor.w * (1 - DiffuseIntensity * lightIntensity));
	output.Color = saturate(FColor);
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    return saturate(input.Color * AmbientIntensity + float4(0,0,0,input.Color.w*(1-AmbientIntensity)));
}

technique Ambient
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
