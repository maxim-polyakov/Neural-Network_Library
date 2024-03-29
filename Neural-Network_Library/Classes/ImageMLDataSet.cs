﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural_Network_Library
{
    public class ImageMLDataSet : BasicMLDataSet
    {
        /// <summary>
        /// Error message to inform the caller that only ImageNeuralData objects can
        /// be used with this collection.
        /// </summary>
        public const String MUST_USE_IMAGE =
            "This data set only supports ImageNeuralData or Image objects.";

        /// <summary>
        /// The downsampler to use.
        /// </summary>
        private readonly IDownSample downsampler;

        /// <summary>
        /// Should the bounds be found and cropped.
        /// </summary>
        private readonly bool findBounds;

        /// <summary>
        /// The high value to normalize to.
        /// </summary>
        private readonly double hi;

        /// <summary>
        /// The low value to normalize to.
        /// </summary>
        private readonly double lo;

        /// <summary>
        /// The height to downsample to.
        /// </summary>
        private int height;

        /// <summary>
        /// The width to downsample to.
        /// </summary>
        private int width;


        /// <summary>
        /// Construct this class with the specified downsampler.
        /// </summary>
        /// <param name="downsampler">The downsampler to use.</param>
        /// <param name="findBounds">Should the bounds be found and clipped.</param>
        /// <param name="hi">The high value to normalize to.</param>
        /// <param name="lo">The low value to normalize to.</param>
        public ImageMLDataSet(IDownSample downsampler,
                              bool findBounds, double hi, double lo)
        {
            this.downsampler = downsampler;
            this.findBounds = findBounds;
            height = -1;
            width = -1;
            this.hi = hi;
            this.lo = lo;
        }

        /// <summary>
        /// The height.
        /// </summary>
        public int Height
        {
            get { return height; }
        }

        /// <summary>
        /// The width.
        /// </summary>
        public int Width
        {
            get { return width; }
        }


        /// <summary>
        /// Add the specified data, must be an ImageNeuralData class.
        /// </summary>
        /// <param name="data">The data The object to add.</param>
        public override void Add(IMLData data)
        {
            if (!(data is ImageMLData))
            {
                throw new NeuralNetworkError(MUST_USE_IMAGE);
            }

            base.Add(data);
        }

        /// <summary>
        /// Add the specified input and ideal object to the collection.
        /// </summary>
        /// <param name="inputData">The image to train with.</param>
        /// <param name="idealData">The expected otuput form this image.</param>
        public override void Add(IMLData inputData, IMLData idealData)
        {
            if (!(inputData is ImageMLData))
            {
                throw new NeuralNetworkError(MUST_USE_IMAGE);
            }

            base.Add(inputData, idealData);
        }

        /// <summary>
        /// Add input and expected output. This is used for supervised training.
        /// </summary>
        /// <param name="inputData">The input data to train on.</param>
        public override void Add(IMLDataPair inputData)
        {
            if (!(inputData.Input is ImageMLData))
            {
                throw new NeuralNetworkError(MUST_USE_IMAGE);
            }

            base.Add(inputData);
        }


        /// <summary>
        /// Downsample all images and generate training data.
        /// </summary>
        /// <param name="height">The height to downsample to.</param>
        /// <param name="width">The width to downsample to.</param>
        public void Downsample(int height, int width)
        {
            this.height = height;
            this.width = width;

            foreach (IMLDataPair pair in this)
            {
                if (!(pair.Input is ImageMLData))
                {
                    throw new NeuralNetworkError(
                        "Invalid class type found in ImageNeuralDataSet, only "
                        + "ImageNeuralData items are allowed.");
                }

                var input = (ImageMLData)pair.Input;
                input.Downsample(downsampler, findBounds, height, width,
                                 hi, lo);
            }
        }
    }
}
