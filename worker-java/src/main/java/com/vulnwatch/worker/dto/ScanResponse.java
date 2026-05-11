package com.vulnwatch.worker.dto;

import java.util.ArrayList;
import java.util.List;
import java.util.UUID;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@Builder
@AllArgsConstructor
@NoArgsConstructor
public class ScanResponse {
  private UUID scanId;
  private List<FindingResponse> findings = new ArrayList<>();
}
