using UnityEngine;
using Verse;

namespace RATS;

public class Graphic_Zoomer : Graphic_Single
{
    public Thing Parent;
    public override Material MatSingle => RATS_Shaders.ZoomMat;

    public override Material MatWest => RATS_Shaders.ZoomMat;

    public override Material MatSouth => RATS_Shaders.ZoomMat;

    public override Material MatEast => RATS_Shaders.ZoomMat;

    public override Material MatNorth => RATS_Shaders.ZoomMat;

    public override Material MatAt(Rot4 rot, Thing thing = null)
    {
        return RATS_Shaders.ZoomMat;
    }

    public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
    {
        if (Parent != null)
        {
            loc = Parent.DrawPos;
            rot = Parent.Rotation;
        }

        loc.y = AltitudeLayer.MetaOverlays.AltitudeFor();

        base.DrawWorker(loc, rot, thingDef, thing, extraRotation);
    }
}
