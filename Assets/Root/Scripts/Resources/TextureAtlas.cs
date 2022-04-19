using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

[XmlRoot("TextureAtlas")]
public class TextureAtlas {

    [XmlAttribute("imagePath")]
    public string imagePath;

    [XmlAttribute("width")]
    public int width;

    [XmlAttribute("height")]
    public int height;

    [XmlElement("sprite")]
    public List<TextureAtlasSprite> Sprites = new List<TextureAtlasSprite>();

    public static TextureAtlas ParseFromFile(string text) {
        XmlSerializer serializer = new XmlSerializer(typeof(TextureAtlas));
        return serializer.Deserialize(new StringReader(text)) as TextureAtlas;
    }

    public override string ToString() {
        return string.Format(
            "imagePath: {0}; width: {1}; height: {2};",
            this.imagePath,
            this.width,
            this.height
        );
    }

}
