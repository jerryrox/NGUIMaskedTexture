using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Inspector class used to edit UIMaskedTextures.
/// </summary>

[CanEditMultipleObjects]
[CustomEditor(typeof(UIMaskedTexture), true)]
public class UIMaskedTextureInspector : UIBasicSpriteEditor  {
	
	UIMaskedTexture mTex;


	protected override void OnEnable () {
		base.OnEnable();
		mTex = target as UIMaskedTexture;
	}

	protected override bool ShouldDrawProperties () {
		if (target == null)
			return false;
		
		SerializedProperty sp = NGUIEditorTools.DrawProperty("Texture", serializedObject, "mTexture");
		if (sp != null) {
			var tex = sp.objectReferenceValue as Texture;
			mTex.mainTexture = tex;
			NGUISettings.texture = tex;
		}

		sp = NGUIEditorTools.DrawProperty("Mask Texture", serializedObject, "mMaskTexture");
		if (sp != null)
			mTex.maskTexture = sp.objectReferenceValue as Texture;
		//NGUIEditorTools.DrawProperty("Material (Test)", serializedObject, "mMat");

		sp = NGUIEditorTools.DrawProperty("Rebuild Material", serializedObject, "mRebuildMaterial");
		bool rebuildMat = sp.boolValue;
		mTex.isRebuildMaterial = rebuildMat;
		if(!rebuildMat) {
			EditorGUILayout.HelpBox("If this flag is disabled, this component will share its material along with other duplicated or prefab instances.", MessageType.Info);
		}

		EditorGUI.BeginDisabledGroup(mTex == null || mTex.mainTexture == null || serializedObject.isEditingMultipleObjects);

		NGUIEditorTools.DrawRectProperty("UV Rect", serializedObject, "mRect");
		sp = NGUIEditorTools.DrawProperty("Fix Mask UV", serializedObject, "mFixMaskUV");
		bool fixMaskUV = sp.boolValue;
		mTex.fixMaskUV = fixMaskUV;
		if(!fixMaskUV) {
			EditorGUILayout.HelpBox("If this flag is disabled, the mask area will be affected by UV Rect settings.", MessageType.Info);
		}

		sp = serializedObject.FindProperty("mFixedAspect");
		bool before = sp.boolValue;
		NGUIEditorTools.DrawProperty("Fixed Aspect", sp);
		if (sp.boolValue != before)
			(target as UIWidget).drawRegion = new Vector4(0f, 0f, 1f, 1f);

		if (sp.boolValue) {
			EditorGUILayout.HelpBox("Note that Fixed Aspect mode is not compatible with Draw Region modifications done by sliders and progress bars.", MessageType.Info);
		}

		EditorGUI.EndDisabledGroup();
		return true;
	}

	/// <summary>
	/// Allow the texture to be previewed.
	/// </summary>
	public override bool HasPreviewGUI () {
		return (Selection.activeGameObject == null || Selection.gameObjects.Length == 1) &&
			(mTex != null) && (mTex.mainTexture as Texture2D != null);
	}

	/// <summary>
	/// Draw the sprite preview.
	/// </summary>
	public override void OnPreviewGUI (Rect rect, GUIStyle background) {
		Texture2D tex = mTex.mainTexture as Texture2D;

		if (tex != null) {
			Rect tc = mTex.uvRect;
			tc.xMin *= tex.width;
			tc.xMax *= tex.width;
			tc.yMin *= tex.height;
			tc.yMax *= tex.height;

			Vector4 border = mTex.border;

			NGUIEditorTools.DrawSprite(tex, rect, mTex.color, mTex.material,
				Mathf.RoundToInt(tc.x),
				Mathf.RoundToInt(tex.height - tc.y - tc.height),
				Mathf.RoundToInt(tc.width),
				Mathf.RoundToInt(tc.height),
				Mathf.RoundToInt(border.x),
				Mathf.RoundToInt(border.y),
				Mathf.RoundToInt(border.z),
				Mathf.RoundToInt(border.w));
		}
	}
}
