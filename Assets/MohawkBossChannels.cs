using UnityEngine;
using SBR;

public class MohawkBossChannels : CharacterChannels {
    public MohawkBossChannels() {
        RegisterInputChannel("doAttack", null, true);

    }
    

    public string doAttack {
        get {
            return GetInput<string>("doAttack");
        }

        set {
            SetInput("doAttack", value);
        }
    }

}
