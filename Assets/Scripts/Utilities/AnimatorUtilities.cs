using UnityEngine;

public static class AnimatorUtilities
{
	public static readonly int SHOW_TRIGGER = Animator.StringToHash("Show");
	public static readonly int HIDE_TRIGGER = Animator.StringToHash("Hide");
	public static readonly int START_TRIGGER = Animator.StringToHash("Start");
	public static readonly int STOP_TRIGGER = Animator.StringToHash("Stop");
	public static readonly int RESET_TRIGGER = Animator.StringToHash("Reset");
	public static readonly int IDLE_TRIGGER = Animator.StringToHash("Idle");
	public static readonly int PIECE_BOOL = Animator.StringToHash("Piece");
}