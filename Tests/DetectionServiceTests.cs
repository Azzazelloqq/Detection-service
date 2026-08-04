using System.Collections.Generic;
using Azzazelloqq.DetectionService.Source;
using NUnit.Framework;
using UnityEngine;
using Service = Azzazelloqq.DetectionService.Source.DetectionService;

namespace Azzazelloqq.DetectionService.Tests
{
public sealed class DetectionServiceTests
{
	private static readonly Vector3 ObserverPosition = Vector3.zero;
	private const float ViewAngle = 90f;
	private const float ViewDistance = 20f;

	[Test]
	public void DetectObjectsInView_ReturnedResultIsOwnedByCaller()
	{
		var service = new Service(5f);
		var forward = new Detectable(new Vector3(0f, 0f, 5f));
		var backward = new Detectable(new Vector3(0f, 0f, -5f));
		service.RegisterObject(forward);
		service.RegisterObject(backward);

		var firstResult = service.DetectObjectsInView(
			ObserverPosition, Vector3.forward, ViewAngle, ViewDistance, 0);
		var secondResult = service.DetectObjectsInView(
			ObserverPosition, Vector3.back, ViewAngle, ViewDistance, 0);

		CollectionAssert.AreEqual(new[] { forward }, firstResult);
		CollectionAssert.AreEqual(new[] { backward }, secondResult);
	}

	[Test]
	public void DetectObjectsInView_WithResultBuffer_ClearsAndReusesBuffer()
	{
		var service = new Service(5f);
		var stale = new Detectable(Vector3.left);
		var detected = new Detectable(new Vector3(0f, 0f, 5f));
		service.RegisterObject(detected);
		var result = new List<IDetectable> { stale };

		service.DetectObjectsInView(
			ObserverPosition, Vector3.forward, ViewAngle, ViewDistance, 0, result);

		CollectionAssert.AreEqual(new[] { detected }, result);
	}

	[Test]
	public void UpdateObjectPosition_MovesObjectBetweenGridCells()
	{
		var service = new Service(5f);
		var detectable = new Detectable(new Vector3(0f, 0f, 5f));
		service.RegisterObject(detectable);
		var oldPosition = detectable.Position;
		detectable.Position = new Vector3(0f, 0f, -5f);

		service.UpdateObjectPosition(detectable, oldPosition);

		Assert.IsEmpty(service.DetectObjectsInView(
			ObserverPosition, Vector3.forward, ViewAngle, ViewDistance, 0));
		CollectionAssert.AreEqual(
			new[] { detectable },
			service.DetectObjectsInView(
				ObserverPosition, Vector3.back, ViewAngle, ViewDistance, 0));
	}

	private sealed class Detectable : IDetectable
	{
		public Detectable(Vector3 position)
		{
			Position = position;
		}

		public Vector3 Position { get; set; }
		public bool IsDead { get; set; }
	}
}
}
