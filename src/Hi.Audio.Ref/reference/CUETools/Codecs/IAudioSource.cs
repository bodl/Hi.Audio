﻿namespace Hi.Audio.Ref.CUETools.Codecs
{
	public interface IAudioSource
	{
		AudioPCMConfig PCM { get; }
		string Path { get; }

		long Length { get; }
		long Position { get; set; }
		long Remaining { get; }

		int Read(AudioBuffer buffer, int maxLength);
		void Close();
	}
}
