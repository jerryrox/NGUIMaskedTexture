﻿using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// A UITexture component with alpha masking shader applied.
/// Most contents are copied directly from UITexture.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/NGUI Masked Texture")]
public class UIMaskedTexture : UIBasicSprite {
	
	[HideInInspector][SerializeField] Rect mRect = new Rect(0f, 0f, 1f, 1f);
	[HideInInspector][SerializeField] Texture mTexture;
	[HideInInspector][SerializeField] Shader mShader;
	[HideInInspector][SerializeField] Vector4 mBorder = Vector4.zero;
	[HideInInspector][SerializeField] bool mFixedAspect = false;

	[HideInInspector][SerializeField] Texture mMaskTexture;

	/// <summary>
	/// Texture used by the UIMaskedTexture. You can set it directly, without the need to specify a material.
	/// </summary>
	public override Texture mainTexture {
		get {
			return mTexture;
		}
		set {
			if (mTexture != value) {
				RemoveFromPanel();
				mTexture = value;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Texture used by UIMaskedTexture for masking the main texture.
	/// </summary>
	public Texture maskTexture {
		get {
			return mMaskTexture;
		}
		set {
			if(mMaskTexture != value) {
				RemoveFromPanel();
				RemoveMaterial();
				mMaskTexture = value;
				UpdateMaterial();
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Material used by the widget.
	/// </summary>
	public override Material material {
		get {
			UpdateMaterial();
			return mMat;
		}
		set {
			RemoveFromPanel();
			mMat = value;
			MarkAsChanged();
		}
	}

	/// <summary>
	/// Shader used by the texture when creating a dynamic material (when the texture was specified, but the material was not).
	/// </summary>
	public override Shader shader {
		get {
			if (mShader == null) {
				mShader = Shader.Find("Unlit/Transparent Masked");
				UpdateMaterial();
			}
			return mShader;
		}
		set {
			if (mShader != value) {
				RemoveFromPanel();
				RemoveMaterial();
				mShader = value;
				UpdateMaterial();
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Whether the texture is using a premultiplied alpha material.
	/// </summary>
	public override bool premultipliedAlpha {
		get { return false; }
	}


	/// <summary>
	/// Sprite's border. X = left, Y = bottom, Z = right, W = top.
	/// </summary>
	public override Vector4 border {
		get {
			return mBorder;
		}
		set {
			if (mBorder != value) {
				mBorder = value;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// UV rectangle used by the texture.
	/// </summary>

	public Rect uvRect {
		get {
			return mRect;
		}
		set {
			if (mRect != value) {
				mRect = value;
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Widget's dimensions used for drawing. X = left, Y = bottom, Z = right, W = top.
	/// This function automatically adds 1 pixel on the edge if the texture's dimensions are not even.
	/// It's used to achieve pixel-perfect sprites even when an odd dimension widget happens to be centered.
	/// </summary>
	public override Vector4 drawingDimensions {
		get {
			Vector2 offset = pivotOffset;

			float x0 = -offset.x * mWidth;
			float y0 = -offset.y * mHeight;
			float x1 = x0 + mWidth;
			float y1 = y0 + mHeight;

			if (mTexture != null && mType != UISprite.Type.Tiled) {
				int w = mTexture.width;
				int h = mTexture.height;
				int padRight = 0;
				int padTop = 0;

				float px = 1f;
				float py = 1f;

				if (w > 0 && h > 0 && (mType == UISprite.Type.Simple || mType == UISprite.Type.Filled)) {
					if ((w & 1) != 0) ++padRight;
					if ((h & 1) != 0) ++padTop;

					px = (1f / w) * mWidth;
					py = (1f / h) * mHeight;
				}

				if (mFlip == UISprite.Flip.Horizontally || mFlip == UISprite.Flip.Both)
					x0 += padRight * px;
				else
					x1 -= padRight * px;

				if (mFlip == UISprite.Flip.Vertically || mFlip == UISprite.Flip.Both)
					y0 += padTop * py;
				else
					y1 -= padTop * py;
			}

			float fw, fh;

			if (mFixedAspect) {
				fw = 0f;
				fh = 0f;
			}
			else {
				Vector4 br = border;
				fw = br.x + br.z;
				fh = br.y + br.w;
			}

			float vx = Mathf.Lerp(x0, x1 - fw, mDrawRegion.x);
			float vy = Mathf.Lerp(y0, y1 - fh, mDrawRegion.y);
			float vz = Mathf.Lerp(x0 + fw, x1, mDrawRegion.z);
			float vw = Mathf.Lerp(y0 + fh, y1, mDrawRegion.w);

			return new Vector4(vx, vy, vz, vw);
		}
	}

	/// <summary>
	/// Whether the drawn texture will always maintain a fixed aspect ratio.
	/// This setting is not compatible with drawRegion adjustments (sliders, progress bars, etc).
	/// </summary>
	public bool fixedAspect {
		get {
			return mFixedAspect;
		}
		set {
			if (mFixedAspect != value) {
				mFixedAspect = value;
				mDrawRegion = new Vector4(0f, 0f, 1f, 1f);
				MarkAsChanged();
			}
		}
	}

	/// <summary>
	/// Adjust the scale of the widget to make it pixel-perfect.
	/// </summary>
	public override void MakePixelPerfect () {
		base.MakePixelPerfect();
		if (mType == Type.Tiled)
			return;

		Texture tex = mainTexture;
		if (tex == null)
			return;

		if (mType == Type.Simple || mType == Type.Filled || !hasBorder) {
			if (tex != null) {
				int w = tex.width;
				int h = tex.height;

				if ((w & 1) == 1)
					++w;
				if ((h & 1) == 1)
					++h;

				width = w;
				height = h;
			}
		}
	}

	/// <summary>
	/// Adjust the draw region if the texture is using a fixed aspect ratio.
	/// </summary>
	protected override void OnUpdate () {
		base.OnUpdate();

		if (mFixedAspect) {
			Texture tex = mainTexture;

			if (tex != null) {
				int w = tex.width;
				int h = tex.height;
				if ((w & 1) == 1)
					++w;
				if ((h & 1) == 1)
					++h;
				
				float widgetWidth = mWidth;
				float widgetHeight = mHeight;
				float widgetAspect = widgetWidth / widgetHeight;
				float textureAspect = (float)w / h;

				if (textureAspect < widgetAspect) {
					float x = (widgetWidth - widgetHeight * textureAspect) / widgetWidth * 0.5f;
					drawRegion = new Vector4(x, 0f, 1f - x, 1f);
				}
				else {
					float y = (widgetHeight - widgetWidth / textureAspect) / widgetHeight * 0.5f;
					drawRegion = new Vector4(0f, y, 1f, 1f - y);
				}
			}
		}
	}

	/// <summary>
	/// Virtual function called by the UIPanel that fills the buffers.
	/// </summary>
	public override void OnFill (List<Vector3> verts, List<Vector2> uvs, List<Color> cols) {
		Texture tex = mainTexture;
		if (tex == null)
			return;

		Rect outer = new Rect(mRect.x * tex.width, mRect.y * tex.height, tex.width * mRect.width, tex.height * mRect.height);
		Rect inner = outer;
		Vector4 br = border;
		inner.xMin += br.x;
		inner.yMin += br.y;
		inner.xMax -= br.z;
		inner.yMax -= br.w;

		float w = 1f / tex.width;
		float h = 1f / tex.height;

		outer.xMin *= w;
		outer.xMax *= w;
		outer.yMin *= h;
		outer.yMax *= h;

		inner.xMin *= w;
		inner.xMax *= w;
		inner.yMin *= h;
		inner.yMax *= h;

		int offset = verts.Count;
		Fill(verts, uvs, cols, outer, inner);

		if (onPostFill != null)
			onPostFill(this, offset, verts, uvs, cols);
	}

	void RemoveMaterial() {
		if(mMat != null) {
			#if UNITY_EDITOR
			if(EditorApplication.isPlaying)
				Destroy(mMat);
			#else
			Destroy(mMat);
			#endif
			mMat = null;
		}
	}

	void CreateMaterial() {
		if(mMat == null)
			mMat = new Material(mShader ?? Shader.Find("Unlit/Transparent Masked"));
	}

	void UpdateMaterial() {
		CreateMaterial();

		mMat.SetTexture("_Mask", mMaskTexture);
	}
}
