package com.vulnwatch.worker.models;

import com.vulnwatch.worker.enums.TargetType;
import io.swagger.v3.oas.annotations.media.Schema;
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
@Schema(description = "Scan job received from C# API via Redis queue")
public class ScanJob {

    @Schema(description = "Unique identifier for this scan",
            example = "550e8400-e29b-41d4-a716-446655440000",
            requiredMode = Schema.RequiredMode.REQUIRED)
    private UUID scanId;

    @Schema(description = "Target types to scan (can be multiple, e.g., DOMAIN and REPOSITORY)",
            example = "[\"DOMAIN\"]",
            allowableValues = {"DOMAIN", "REPOSITORY"},
            requiredMode = Schema.RequiredMode.REQUIRED)
    private List<TargetType> targetType;

    @Schema(description = "Domain ID (required if targetType contains DOMAIN)",
            example = "550e8400-e29b-41d4-a716-446655440001")
    private UUID domainId;

    @Schema(description = "Domain name (required if targetType contains DOMAIN)",
            example = "example.com")
    private String domainName;

    @Schema(description = "Repository ID (required if targetType contains REPOSITORY)",
            example = "123456789")
    private Long repoId;

    @Schema(description = "Repository full name (required if targetType contains REPOSITORY)",
            example = "owner/repo-name")
    private String repoFullName;

    @Schema(description = "When the scan job was created",
            example = "2026-05-11T10:00:00Z")
    private Instant createdAt;


}


