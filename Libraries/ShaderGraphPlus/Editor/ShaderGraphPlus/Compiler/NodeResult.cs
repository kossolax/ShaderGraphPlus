﻿namespace Editor.ShaderGraphPlus;

public enum ResultType
{ 
	/// <summary>
	/// No Components, just True or False.
	/// </summary>
	Bool,
	/// <summary>
	/// 1 Component
	/// </summary>
	Float,
	/// <summary>
	/// 2 Component's
	/// </summary>
	Vector2,
	/// <summary>
	/// 3 Component's
	/// </summary>
	Vector3,
    /// <summary>
    /// 4 Component's
    /// </summary>
    Color,
    /// <summary>
    /// 4 Component's
    /// </summary>
    Float2x2,
    /// <summary>
    /// 9 Component's
    /// </summary>
    Float3x3,
    /// <summary>
    /// 16 Component's
    /// </summary>
    Float4x4,
	Sampler,
	TextureObject,
	String,
	Gradient,
}

public struct NodeResult : IValid
{
	public delegate NodeResult Func( GraphCompiler compiler );
	public string Code { get; private set; }
	public ResultType ResultType { get; private set; }
	public bool Constant { get; set; }
	public string[] Errors { get; private init; }
	public string[] Warnings { get; private init; }
	public bool IsComponentLess { get; set; }
	public bool IsDepreciated { get; set; }

	public readonly bool IsValid => ResultType > (ResultType)(-1) && !string.IsNullOrWhiteSpace( Code );

	public readonly string TypeName
	{
		get
		{
			if ( ResultType is ResultType.Float )
			{
				return $"float";
			}
			else if ( ResultType is ResultType.Vector2 or ResultType.Vector3 or ResultType.Color )
			{
				return $"float{Components()}";
			}
			else if ( ResultType is ResultType.Float2x2 )
			{
				return "float2x2";
			}
			else if ( ResultType is ResultType.Float3x3 )
			{
				return "float3x3";
			}
			else if ( ResultType is ResultType.Float4x4 )
			{
				return "float4x4";
			}
			else if ( ResultType is ResultType.Bool )
			{
				return "bool";
			}
			else if ( ResultType is ResultType.String )
			{
				return "";
			}
            else if (ResultType is ResultType.Gradient)
            {
                return "Gradient";
            }

            return "float";
		}
	}

	public readonly Type ComponentType
	{
		get
		{
			return ResultType switch
			{
				ResultType.Bool => typeof( bool ),
				ResultType.Float => typeof( float ),
				ResultType.Vector2 => typeof( Vector2 ),
				ResultType.Vector3 => typeof( Vector3 ),
				ResultType.Color => typeof( Color ),
				ResultType.Float2x2 => typeof( Float2x2 ),
				ResultType.Float3x3 => typeof( Float3x3 ),
				ResultType.Float4x4 => typeof( Float4x4 ),
				ResultType.Sampler => typeof( Sampler ),
				ResultType.TextureObject => typeof( TextureObject ),
				ResultType.String => typeof( string ), // Generic Result
				ResultType.Gradient => typeof( Gradient ),
				_ => throw new System.NotImplementedException(),
			};
		}
	}

	public NodeResult( ResultType resulttype, string code, bool constant = false, bool iscomponentless = false)
	{
		ResultType = resulttype;
		Code = code;
		Constant = constant;
		IsComponentLess = iscomponentless;
	}

	public static NodeResult Error( params string[] errors ) => new() { Errors = errors };
	public static NodeResult Warning( params string[] warnings ) => new() { Warnings = warnings };
	public static NodeResult MissingInput( string name ) => Error( $"Missing required input '{name}'." );
	public static NodeResult Depreciated( (string,string) name ) => Error( $"'{name.Item1}' is depreciated please use '{name.Item2} instead'." );

	/// <summary>
	/// "Cast" this result to different float types
	/// </summary>
	public string Cast( int components, float defaultValue = 0.0f )
	{
		if ( Components() == components)
            return Code;

		if ( Components() > components )
        {
			return $"{Code}.{"xyzw"[..components]}";
		}
		else if ( ResultType == ResultType.Float )
		{
			return $"float{components}( {string.Join( ", ", Enumerable.Repeat( Code, components ) )} )";
		}
		else
		{
			return $"float{components}( {Code}, {string.Join( ", ", Enumerable.Repeat( $"{defaultValue}", (ResultType)components - ResultType ) )} )";
		}
	}

	public readonly int Components()
	{
        int components = 0;

		switch (ResultType)
		{
            case ResultType.Float:
                components = 1;
                break;
            case ResultType.Vector2:
                components = 2;
                break;
            case ResultType.Vector3:
                components = 3;
                break;
            case ResultType.Color:
                components = 4;
                break;
            case ResultType.Float2x2:
                components = 4;
                break;
            case ResultType.Float3x3:
                components = 9;
                break;
            case ResultType.Float4x4:
                components = 16;
				break;
			default:
                Log.Warning($"Result type: '{ResultType}' has no components.");
			break;
		}

		return components;
    }

	public override readonly string ToString()
	{
		return Code;
	}
}
