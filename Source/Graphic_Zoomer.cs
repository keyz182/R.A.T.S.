using UnityEngine;
using Verse;

namespace RATS;

public class Graphic_Zoomer : Graphic_Single
{
    public override Material MatSingle => RATS_Shaders.ZoomMat;

    public override Material MatWest => RATS_Shaders.ZoomMat;

    public override Material MatSouth => RATS_Shaders.ZoomMat;

    public override Material MatEast => RATS_Shaders.ZoomMat;

    public override Material MatNorth => RATS_Shaders.ZoomMat;
}
