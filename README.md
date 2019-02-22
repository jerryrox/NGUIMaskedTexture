# NGUIMaskedTexture

UIMaskedTexture is a component which is based on the UITexture class, with the objective to provide an easy method of alpha-masking any specified texture.

### Motivation
Like the Unity's built-in RawImage component, UITexture provides a convenient way of rendering a Texture object directly on the screen. It also exposes the Shader and Material properties in the inspector to allow further customization.
  Achieving a simple custom effect such as additive blending is an easy task; simply change the Shader property in the inspector then it's done!
  In contrast, masking an image using UITexture is quite complicated and often causes misunderstandings for beginners. First, you must create a new material with "Unlit/Transparent Masked" shader. Then you need to specify the masking texture for _Mask variable. Finally, set the material's reference on an UITexture component along with the main texture you want to mask.
  The problem here is that you must manually create a new material each time a unique mask is required in your game. Furthermore, changing the mask in editor doesn't show any change until you manually toggle off/on your widgets. This also applies during runtime, as UITexture doesn't know if any change has been made on its material.
  
  This component was created in hope to solve any discomfort that arises when using UITexture for masking.

### How to use
1. Create an object and attach the UIMaskedTexture component.
2. Specify the Main Texture and the Mask Texture (if required).
3. Done!
  
  For additional details, look <https://blog.naver.com/reisenmoe/221310332773>
  See how changing the mask immediately shows changes in editor.
  
### Draw calls
Let's assume there is only one panel and two UIMaskedTexture objects in the scene.
* Same Main texture && Same Mask texture => 1 draw call
* Same Main texture && Different Mask texture => 2 draw calls
* Different Main texture && Same Mask texture => 2 draw calls
* Different Main texture && Different Mask texture => 2 draw calls
  This means you'll only get 1 draw call for any UIMaskedTexture instance with unique Main/Mask texture, just like the normal UITexture.
  
  Also read <https://blog.naver.com/reisenmoe/221309822030> for an NGUI draw call optimization technique.
  
### To-Do
* Fix mask textures applying to all duplicated instances during edit mode.
  
### 2019-02-22
* Added the option to fix UV rect of the mask texture. <https://blog.naver.com/reisenmoe/221472281974>
  
#### License
MIT
