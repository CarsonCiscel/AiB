using System.Collections.Generic;
using UnityEngine;

namespace BreathLib
{
	/// <summary>
	/// Data class for holding and manipulating Aggregate Breath Samples.
	/// Contains a samples for the most recent in-the-moment data.
	/// </summary>
	public class BreathStream
	{
		/// <summary>
		/// Creates a new AggregateData object with the given buffer size for the samples.
		/// </summary>
		/// <param name="size">Fixed size of the sames</param>
		public BreathStream(int size)
		{
			samples = new BreathSample[size];
			lastPut = -1;
			TotalSamples = 0;
		}

		/// <summary>
		/// High Quality Timeline: A buffer for history breath sample. Readonly due to fixed size.
		/// This is a circular buffer that has a fixed size.
		/// The oldest data is removed when the buffer is full.
		/// </summary>
		private readonly BreathSample[] samples;

		/// <summary>
		/// The index for the end of the timeline.
		/// </summary>
		private int lastPut;
		/// <summary>
		/// Total number of samples that have been recorded into the samples.
		/// This is not the same as the size of the samples once the buffer is full (and overwrites data).
		/// </summary>
		public ulong TotalSamples { get; private set; }

		/// <summary>
		/// Adds a new data sample to the samples, overwriting the oldest data if the buffer is full.
		/// </summary>
		/// <param name="data">New element to add to the samples</param>
		public void Enqueue(BreathSample data)
		{
			lastPut = (lastPut + 1) % samples.Length;
			TotalSamples++;
			samples[lastPut] = data;
		}

		/// <summary>
		/// The size of the stream, which is the number of samples that can fix in the stream.
		/// </summary>
		public int Length => samples.Length;

		/// <summary>
		/// Helper for getting the last data enqueued into the samples.
		/// </summary>
		/// <returns>The last Breath sample data that was added to the samples</returns>
		public BreathSample Last => samples[lastPut];

		public BreathSample? this[ulong index]
		{
			get
			{
				if (index >= TotalSamples || index + (ulong)samples.Length < TotalSamples)
					return null;
				else
					return samples[(int)(index % (ulong)samples.Length)];
			}
		}

		/// <summary>
		/// Copies the samples into a new array, which is ordered from oldest to newest.
		/// Useful for getting a snapshot of the samples at a specific time, without having to worry about circular indexing.
		/// </summary>
		/// <param name="buffer">The array that will be filled with BreathSample from the latest 'buffer.Length' samples.</param>
		public void GetSamples(BreathSample[] buffer)
		{
			if (buffer.Length > samples.Length)
			{
				Debug.LogError("Buffer is too large. Please use a buffer of size " + samples.Length + " or smaller.");
				return;
			}

			int index = lastPut + 1 - buffer.Length;
			if (index < 0) index += samples.Length;

			for (int i = 0; i < buffer.Length; i++, index = (index + 1) % samples.Length)
			{
				buffer[i] = samples[index];
			}
		}
	}
}
