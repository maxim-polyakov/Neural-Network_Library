using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class ImageMLData : BasicMLData
    {
        /// <summary>
        /// Construct an object based on an image.
        /// </summary>
        /// <param name="image">The image to use.</param>
        public ImageMLData(Bitmap image)
            : base(1)
        {
            Image = image;
        }

        /// <summary>
        /// The image associated with this class.
        /// </summary>
        public Bitmap Image { get; set; }


        /// <summary>
        /// Downsample, and copy, the image contents into the data of this object.
        /// Calling this method has no effect on the image, as the same image can be
        /// downsampled multiple times to different resolutions.
        /// </summary>
        /// <param name="downsampler">The downsampler object to use.</param>
        /// <param name="findBounds">Should the bounds be located and cropped.</param>
        /// <param name="height">The height to downsample to.</param>
        /// <param name="width">The width to downsample to.</param>
        /// <param name="hi">The high value to normalize to.</param>
        /// <param name="lo">The low value to normalize to.</param>
        public void Downsample(IDownSample downsampler,
                               bool findBounds, int height, int width,
                               double hi, double lo)
        {

        }

        /// <summary>
        /// Return a string representation of this object.
        /// </summary>
        /// <returns>The string form of this object.</returns>
        public override String ToString()
        {
            var builder = new StringBuilder("[ImageNeuralData:");
            for (int i = 0; i < Data.Length; i++)
            {
                if (i != 0)
                {
                    builder.Append(',');
                }
                builder.Append(Data[i]);
            }
            builder.Append("]");
            return builder.ToString();
        }
    }
}
