# Shape Pinball

*by Christian Bruccoleri*

## Ideas to Implement


### Features

- UI
	- health, energy, score
	- Game Over and Restart

- Objects that are being hit should generate energy "pills". These are attracted by the blackhole only.

- Add sound effects

- Wanderers should have some sense of purpose (basic AI): e.g. seek energy sources. The black hole is an energy source, but it reduces health. Health is restored by the health pills released by wanderers and  static obstacles when hit.

- Add music

- Gamma Ray burst from the Black Hole can kill anything in a random direction, unless in a safe zone

- [DONE] Wanderers are attracted by the black hole, but do not fall in

- [DONE] Visualize Vector field.

- [DONE] Each object attracts or repels with a small force.


### Visual and Art

- Add noise texture to mouse ripples

- Animations for coming to life and dying

- [DONE] Show mouse clicks with a ripple, like waves in waters that quickly fade

- [DONE] Implemented Anti Aliasing on Black Hole

- [DONE] Lights and Post-Processing Effects.

- [DONE] Shader for Black Hole swirling effect

### Bugs

- Fix score points being added for Wanderers hitting static shapes.

- [DONE] Fix particles system: emitting cone on +Z axis (3d)



## Adding Post processing effects in Unity (Default Rendering Pipeline)

1. Make sure that the package Post Processing v2 (3.1) is installed in Package Manager

2. Add a component to the main camera. From the menu: Component > Rendering > Post-Process Volume. This is not a volume though, it is a component to make the camera "aware" that you may be applying post-processing to volumes. Note: you must use the main menu, the contextual menu (right click) does not show this option because it is a Component, not a GameObject.

3. Now we must create a GameObject in the Hiearchy to actually hold a volume (i.e. an object with a Collider and a Post-Process Volume Component).
Add with GameObject > 3D Object > Post-Process Volume. Adjust the collider box to the volume you want to post-process, then create a new Post-Process Volume profile (See the Inspector for this component).

4. Now you can add the effects. Regarding the Volume effects, these make sense in 3D. In 2D, do not bother and set the volume to Global. This is because the effect is applied to the camera only when it (or another triggering object) is in that volume. Thus, with an orthographic camera, this does not work well at all.

https://docs.unity3d.com/Packages/com.unity.postprocessing@3.1/manual/Quick-start.html
https://www.youtube.com/watch?v=9tjYz6Ab0oc&ab_channel=Brackeys [however, uses URP]



## Adding an Image Effect aka Render Image

Turns out that making post-processing effects in Unity with the standard rendering pipeline is very easy: just override the OnRenderImage event handler in a script attached to the active camera. All you need is: 1) A material to which an "Image Effect" shader is attached to, 2) Call the function `Graphics.Blit(RenderTexture, RenderTexture, Material)` at the end of the event. The material also allows you to set all the parameters to the shader. It is the shader that does the hard work.


## Here is a clear example on how to implement Anti-Aliasing with fwidth()

//smooth version of step
float aaStep(float compValue, float gradient){
	float halfChange = fwidth(gradient) / 2;
	//base the range of the inverse lerp on the change over one pixel
	float lowerEdge = compValue - halfChange;
	float upperEdge = compValue + halfChange;
	//do the inverse interpolation
	float stepped = (gradient - lowerEdge) / (upperEdge - lowerEdge);
	stepped = saturate(stepped);
	//smoothstep version here would be `smoothstep(lowerEdge, upperEdge, gradient)`
	return stepped;
}