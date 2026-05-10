package com.vulnwatch.worker.models;

import java.util.List;
import java.util.UUID;
import lombok.Data;

/**
 * ScanJob model: Matches the JSON structure from the Redis queue. Intern-friendly: This is the
 * "Payload" we receive from the C# API.
 */
@Data
public class ScanJob {
  private UUID scan_id;
  private String domain;
  private List<String> scan_type;
  private String requested_by;
}
