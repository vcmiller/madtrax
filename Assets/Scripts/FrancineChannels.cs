using UnityEngine;
using SBR;

public class FrancineChannels : CharacterChannels {
    public FrancineChannels() {
        RegisterInputChannel("aim", new Vector3(0, 0, 0), true);
        RegisterInputChannel("movement2", new Vector3(0, 0, 0), true);

    }
    

    public Vector3 aim {
        get {
            return GetInput<Vector3>("aim");
        }

        set {
            SetVector("aim", value);
        }
    }

    public Vector3 movement2 {
        get {
            return GetInput<Vector3>("movement2");
        }

        set {
            SetVector("movement2", value);
        }
    }

}
