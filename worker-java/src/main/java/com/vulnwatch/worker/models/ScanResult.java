package com.vulnwatch.worker.models;

import com.vulnwatch.worker.enums.SurfaceType;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.time.Instant;
import java.util.Map;
import java.util.UUID;

/**
 * Raw result from a single scanner before AI enrichment.
 * This is what each scanner returns to the orchestrator.
 * Other team members implement scanners that return this object.
 */
@Data
@Builder
@NoArgsConstructor
@AllArgsConstructor
public class ScanResult {

    private UUID scanId;
    private String scannerName;
    private SurfaceType surface;
    private Instant timestamp;
    private boolean success;
    private String errorMessage;
    private Map<String, Object> rawData;

    public String toJson() {
        return null;
    }
}
