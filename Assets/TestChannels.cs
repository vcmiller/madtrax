using UnityEngine;
using SBR;

public class TestChannels : CharacterChannels {
    public TestChannels() {
        RegisterInputChannel("target", new Vector3(0, 0, 0), false);
        RegisterInputChannel("attack", false, true);

    }
    

    public Vector3 target {
        get {
            return GetInput<Vector3>("target");
        }

        set {
            SetVector("target", value);
        }
    }

    public bool attack {
        get {
            return GetInput<bool>("attack");
        }

        set {
            SetInput("attack", value);
        }
    }

}
