package com.vulnwatch.worker.queue;

import java.util.UUID;

public class RedisResultPublisher {
    public void publishCompletion(UUID scanId, String completed, int securityScore) {
    }

    public void publishFailure(UUID scanId, String message) {
    }
}
