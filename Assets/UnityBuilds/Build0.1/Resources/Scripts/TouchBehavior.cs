using UnityEngine;
using System.Collections;

public interface TouchBehavior {

	// Use this for initialization
	bool isFling { get; set; }
	void ResolveTouchBehavior();
}
