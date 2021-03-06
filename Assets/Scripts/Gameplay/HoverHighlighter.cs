using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverHighlighter : MonoBehaviour, SelectionManager.IHoverable /* It has to be IHoverable so that the SelectionManager can detect it*/ {

	// Object which is enabled when we hover over this object
	public GameObject highlightObject;

	// De/Register this object with the hover manager when it is dis/enabled.
	void OnEnable(){ SelectionManager.hoverChangedEvent += OnHoverChanged; }
	void OnDisable(){ SelectionManager.hoverChangedEvent -= OnHoverChanged; }

	// Callback which changes the hovered state of the object
	public void OnHoverChanged(GameObject newHover){
		if(newHover == gameObject)
			requestHoverEnable(true);
		else if(SelectionManager.instance.hovered == gameObject && newHover != gameObject)
			requestHoverEnable(false);
	}

	// Function which enables the hovering logic
	void requestHoverEnable(bool enable){
		highlightObject.SetActive(enable);
	}
}
