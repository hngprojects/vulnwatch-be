package com.vulnwatch.worker.models;

import com.vulnwatch.worker.enums.TargetType;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.time.Instant;
import java.util.List;
import java.util.UUID;

/**
 * ScanJob model: Matches the JSON structure from the Redis queue.
 * Intern-friendly: This is the "Payload" we receive from the C# API.
 */
@Data
@Builder
@NoArgsConstructor
@AllArgsConstructor
public class ScanJob {
    private UUID scanId;
    private UUID userId;
    private UUID idempotencyKey;
    private TargetType targetType;
    private UUID domainId;
    private String domainName;
    private Long repoId;
    private String repoFullName;
    private Instant createdAt;
}
