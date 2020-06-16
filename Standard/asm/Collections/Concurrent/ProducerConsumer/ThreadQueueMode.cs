﻿namespace asm.Collections.Concurrent.ProducerConsumer
{
	public enum ThreadQueueMode
	{
		Task,
		DataFlow,
		WaitAndPulse,
		Event,
		BlockingCollection,
		TaskGroup,
		SemaphoreSlim,
		Semaphore,
		Mutex,
		ThresholdTaskGroup
	}
}