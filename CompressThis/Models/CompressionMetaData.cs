using System.Text;

namespace CompressThis.Models
{
    public class CompressionMetaData
    {
        public CompressionMetaData(int blockCount)
        {
            BlockSizes = new int[blockCount];
        }

        public int[] BlockSizes { get; }

        public int MetaDataSize => BlockSizes.Length * 4 + 4;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Compression Info");
            for (int i = 0; i < BlockSizes.Length; i++)
            {
                sb.AppendLine($"Block #{i}:\t{BlockSizes[i]}");
            }

            return sb.ToString();
        }
    }
}