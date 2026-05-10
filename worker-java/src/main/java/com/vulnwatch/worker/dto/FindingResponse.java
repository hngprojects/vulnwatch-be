package com.vulnwatch.worker.dto;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.util.UUID;

@Data
@AllArgsConstructor
@NoArgsConstructor
public class FindingResponse {
    private String surface;
    private String severity;
    private String title;
    private String aiExplanation;
    private String technicalPayload;
    private String remediation;
}
