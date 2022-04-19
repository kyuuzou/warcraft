using System.Xml;
using System.Xml.Serialization;

public class TextureAtlasSprite {

    [XmlAttribute("n")]
    public string n;

    [XmlAttribute("x")]
    public int x;

    [XmlAttribute("y")]
    public int y;

    [XmlAttribute("w")]
    public int w;

    [XmlAttribute("h")]
    public int h;

    [XmlAttribute("pX")]
    public string pX;

    [XmlAttribute("pY")]
    public string pY;

    public override string ToString() {
        return string.Format(
            "n: {0}; x: {1}; y: {2}; w: {3}; h: {4}; pX: {5}; pY: {6};",
            this.n,
            this.x,
            this.y,
            this.w,
            this.h,
            this.pX,
            this.pY
        );
    }
}
