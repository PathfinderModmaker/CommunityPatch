using System;
using System.Collections.Generic;
using UnityModManagerNet;

namespace CommunityPatch
{
	// Token: 0x02000016 RID: 22
	public class Settings : UnityModManager.ModSettings
	{
		// Token: 0x06000055 RID: 85 RVA: 0x00002348 File Offset: 0x00000548
		public override void Save(UnityModManager.ModEntry modEntry)
		{
			UnityModManager.ModSettings.Save<Settings>(this, modEntry);
		}

        // Token: 0x04000040 RID: 64


        public bool useKingdomManagementCustomPortraitFix = true;
        public bool useDialogListenerOnSpeakerNameChangeFix = true;
        public bool useDialogListenerDefaultCompanionCustomPortraitFix = true;







    }
}
