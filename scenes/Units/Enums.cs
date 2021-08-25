using Godot;

public class Enums : Node {
    public enum UnitState {IDLE, MOVE, ATTACK, RELOAD, DEAD};
    // 
    public enum Units {None, Soldier, Civilian}; 

    public enum Faction {Human, Alien}
}