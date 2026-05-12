package com.vulnwatch.worker.queue;

import com.vulnwatch.worker.enums.ScanStatus;
import java.util.UUID;
import org.springframework.stereotype.Component;

@Component
public class RedisResultPublisher {
  public void publishCompletion(UUID scanId, ScanStatus completed, int securityScore) {}

  public void publishFailure(UUID scanId, String message) {}
}
